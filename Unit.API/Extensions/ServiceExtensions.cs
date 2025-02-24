using Amazon.CognitoIdentityProvider;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Extensions.NETCore.Setup;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Unit.API.Configuration;
using Unit.Entities.ConfigurationModels;
using Unit.Repository.Contracts;
using Unit.Repository;
using Unit.Service;
using Unit.Service.Contracts;
using Amazon.S3;

namespace Unit.API.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureLogger(this IServiceCollection services)
            => services.AddSingleton<ILoggerManager, LoggerManager>();

        public static void ConfigureCors(this IServiceCollection services)
            => services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .WithExposedHeaders("X-Pagination"));
            });

        public static void ConfigureServiceManager(this IServiceCollection services)
            => services.AddScoped<IServiceManager, ServiceManager>();

        public static void ConfigureRepositoryManager(this IServiceCollection services)
            => services.AddScoped<IRepositoryManager, RepositoryManager>();

        public static void ConfigureAWSConfiguration(this IServiceCollection services, IConfiguration configuration)
            => services.Configure<AWSConfiguration>(configuration.GetSection("AWS"));

        public static void ConfigureAWS(this IServiceCollection services, IConfiguration configuration)
        {

            var awsConfig = new AWSConfiguration();
            configuration.Bind(awsConfig.Section, awsConfig);

            var awsOptions = configuration.GetAWSOptions();
            var accessKey = awsConfig.AccessKey;
            var secretKey = awsConfig.SecretKey;

            // Khởi tạo thông tin xác thực thủ công
            awsOptions.Credentials = new Amazon.Runtime.BasicAWSCredentials(accessKey, secretKey);

            services.AddDefaultAWSOptions(awsOptions);
            services.AddAWSService<IAmazonDynamoDB>();
            services.AddAWSService<IAmazonCognitoIdentityProvider>();
            services.AddAWSService<IAmazonS3>();
            services.AddScoped<IDynamoDBContext, DynamoDBContext>();
        }

    }
}
