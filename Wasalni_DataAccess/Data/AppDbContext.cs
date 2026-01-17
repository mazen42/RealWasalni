using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using Wasalni_Models;

namespace Wasalni_DataAccess.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Bus> Buses { get; set; }
        public DbSet<BusTrip> BusTrips { get; set; }
        public DbSet<RoutePlan> RoutePlans { get; set; }
        public DbSet<Passenger> Passengers { get; set; }
        public DbSet<TripPoint> TripPoints { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<DriverProfile> DriverProfiles { get; set; }
        public DbSet<RideRequest> RideRequests { get; set; }
        public DbSet<DriverTripRequest> DriverTripRequests { get; set; }

        public AppDbContext(DbContextOptions Options) : base(Options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Bus>().HasKey(x => x.Id);
            builder.Entity<Bus>().Property(x => x.PlateNumber).HasMaxLength(7).IsRequired(true);
            builder.Entity<ApplicationUser>().HasIndex(x => x.Email).IsUnique();
            builder.Entity<ApplicationUser>().OwnsOne(c => c.HomeLocation, loc =>
            {
                loc.Property(c => c.Latitude).HasPrecision(9,4).IsRequired(false);
                loc.Property(c => c.Longitude).HasPrecision(9, 4).IsRequired(false);
            });
            builder.Entity<Passenger>().OwnsOne(c => c.FromLocation, loc =>
            {
                loc.Property(c => c.Latitude).HasPrecision(9, 4).IsRequired(true);
                loc.Property(c => c.Longitude).HasPrecision(9, 4).IsRequired(true);
            });
            builder.Entity<Passenger>().OwnsOne(c => c.ToLocation, loc =>
            {
                loc.Property(c => c.Latitude).HasPrecision(9, 4).IsRequired(true);
                loc.Property(c => c.Longitude).HasPrecision(9, 4).IsRequired(true);
            });
            builder.Entity<BusTrip>().HasOne(x => x.RoutePlan).WithOne(x => x.BusTrip).HasForeignKey<RoutePlan>(x => x.BusTripId).OnDelete(DeleteBehavior.Cascade).IsRequired();
            builder.Entity<ApplicationUser>().HasMany(u => u.Notifications).WithOne(x => x.ApplicationUser).HasForeignKey(n => n.ApplicationUserId).OnDelete(DeleteBehavior.Cascade).IsRequired();
            builder.Entity<RideRequest>().HasMany(c => c.passengers).WithOne(x => x.RideRequest).HasForeignKey(c => c.RideRequestId).OnDelete(DeleteBehavior.NoAction).IsRequired(false);
            builder.Entity<DriverProfile>().HasMany(c => c.TripRequests).WithOne(x => x.Driver).HasForeignKey(c => c.DriverId).OnDelete(DeleteBehavior.NoAction).IsRequired();//this line cause a cycle
            builder.Entity<DriverProfile>().HasMany(x => x.busTrips).WithOne(x => x.DriverProfile).HasForeignKey(x => x.DriverProfileId).OnDelete(DeleteBehavior.NoAction).IsRequired();
            builder.Entity<DriverProfile>().HasOne(x => x.Bus).WithOne(b => b.DriverProfile).HasForeignKey<Bus>(x => x.DriverProfileId).OnDelete(DeleteBehavior.NoAction).IsRequired();
            builder.Entity<DriverProfile>().HasOne(x => x.ApplicationUser).WithOne().HasForeignKey<DriverProfile>(x => x.ApplicationUserId).OnDelete(DeleteBehavior.NoAction).IsRequired();
            builder.Entity<BusTrip>().HasMany(x => x.Passengers).WithOne(x => x.BusTrip).HasForeignKey(x => x.BusTripId).OnDelete(DeleteBehavior.NoAction).IsRequired(false);
            builder.Entity<RoutePlan>().HasMany(x => x.PickUpPoints).WithOne(x => x.RoutePlan).HasForeignKey(x => x.RoutePlanId).OnDelete(DeleteBehavior.NoAction).IsRequired();
            builder.Entity<Passenger>().HasOne(x => x.ApplicationUser).WithOne().HasForeignKey<Passenger>(x => x.ApplicationUserId).IsRequired();


        }
    }
}
