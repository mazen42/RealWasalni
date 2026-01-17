using Microsoft.Extensions.Options;

namespace Wasalni.Options
{
    public class DataBaseOptionsSetup : IConfigureOptions<DataBaseOptions>
    {

        private const string DataBaseOptionsSection = "DataBaseOptions";
        private readonly IConfiguration _configuration;
        public DataBaseOptionsSetup(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void Configure(DataBaseOptions options)
        {
            _configuration.GetSection(DataBaseOptionsSection).Bind(options);
            options.ConnectionString = _configuration.GetConnectionString("DefaultConnection");
        }
    }
}
