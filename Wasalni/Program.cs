using FluentValidation;
using Hangfire;
using HangfireBasicAuthenticationFilter;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Wasalni.Infrastructure.Hubs;
using Wasalni.Infrastructure.Interfaces;
using Wasalni.Infrastructure.Repositories;
using Wasalni.Options;
using Wasalni.Services;
using Wasalni_DataAccess;
using Wasalni_DataAccess.Data;
using Wasalni_Models;
using Wasalni_Models.DTOs;
using Wasalni_Utility;

namespace Wasalni
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly,includeInternalTypes: true);

            //builder.Services.ConfigureOptions<DataBaseOptionsSetup>();
            // -------------------- Database --------------------
            var connectionString = Environment.GetEnvironmentVariable("MyDbConnection");
            builder.Services.AddDbContext<AppDbContext>((ServiceProvider,DbContextOptionsBuilder) =>
            {
                //var dataBaseOptions = ServiceProvider.GetService<IOptions<DataBaseOptions>>()!.Value;
                DbContextOptionsBuilder.UseSqlServer(connectionString, sqlActions =>
                {
                    sqlActions.CommandTimeout(30);
                    sqlActions.EnableRetryOnFailure(3);
                });
                DbContextOptionsBuilder.EnableSensitiveDataLogging(true);
                DbContextOptionsBuilder.EnableDetailedErrors(false);

            });
            builder.Services.AddSignalR();

            builder.Services.AddHangfire((sp, config) =>
            {
                config.UseSimpleAssemblyNameTypeSerializer().UseRecommendedSerializerSettings();
                config.UseSqlServerStorage(connectionString);
            });
            builder.Services.AddHangfireServer();
            builder.Services.AddScoped<IBackGroundJobsServices,BackGroundJobService>();
            builder.Services.AddScoped<IRecurringJobsInitializer, RecurringJobsInitializer>();
            builder.Services.AddScoped<INotificationService, NotificationService>();
            builder.Services.AddHttpClient();
            builder.Services.AddScoped<IDbInitializer, Dbinitializser>();



            // -------------------- Identity --------------------
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();
            builder.Services.AddControllers().AddJsonOptions(o =>
            {
                o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: false));
                o.JsonSerializerOptions.Converters.Add(new TimeOnlyJsonConverter());
            });

            // -------------------- UnitOfWork --------------------
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // -------------------- JWT Authentication --------------------
            var key = Encoding.ASCII.GetBytes(builder.Configuration["JWT:Key"]!);
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            // -------------------- Swagger --------------------
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your valid token."
                });

                c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });
            builder.Services.AddCors(o => o.AddPolicy("AllowAll", p =>
            {
                p.AllowAnyOrigin()
                 .AllowAnyMethod()
                 .AllowAnyHeader();
            }));

            // -------------------- Controllers --------------------
            builder.Services.AddControllers();
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();
            builder.Logging.SetMinimumLevel(LogLevel.Trace);

            var app = builder.Build();

            // -------------------- Pipeline --------------------
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Wasalni API V1");
                    c.RoutePrefix = string.Empty; // Swagger on root "/"
                });
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCors("AllowAll");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapHub<NotificationHub>("/NotificationHub");
            await seedDataAsync();
            using (var scope = app.Services.CreateScope())
            {
                var jobs = scope.ServiceProvider.GetRequiredService<IRecurringJobsInitializer>();
                jobs.DecrementPassengersLeftDaysInAllTripsJob();
                //await jobs.InvitationsRejection();
                jobs.checkTripsDates();
            }
            app.MapControllers();
            app.MapHub<NotificationHub>("/Hubs/NotificationHub");
            app.UseHangfireDashboard("/Jobs",new DashboardOptions
            {
                DisplayStorageConnectionString = false,
                Authorization = new[] {
                    new HangfireCustomBasicAuthenticationFilter
                    {
                        User = "Admin",
                        Pass = "admin123"
                    }
                }

            });
            app.Run();
            async Task seedDataAsync()
            {
                using (var scope = app.Services.CreateScope())
                {
                    var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
                    await dbInitializer.InitializeAsync();
                }
            }
        }
    }
}

