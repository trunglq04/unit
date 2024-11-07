using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

namespace Unit.API.Configuration
{
    public class JwtBearerConfigurationOptions(IConfiguration configuration) : IConfigureNamedOptions<JwtBearerOptions>
    {
        private const string ConfiguratioSectionName = "JwtBearer";
        public void Configure(string? name, JwtBearerOptions options)
        {
            Configure(options);
        }

        public void Configure(JwtBearerOptions options)
        {
            configuration.GetSection(ConfiguratioSectionName).Bind(options);
        }
    }
}
