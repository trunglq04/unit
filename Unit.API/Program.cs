using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NLog;
using Unit.API.ActionFilter;
using Unit.API.Configuration;
using Unit.API.Extensions;
using Unit.Entities.ConfigurationModels;
using Unit.Service;
using Unit.Service.Contracts;

var builder = WebApplication.CreateBuilder(args);

LogManager.Setup().LoadConfigurationFromFile(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));

builder.Services.ConfigureCors();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.ConfigureLogger();
builder.Services.ConfigureServiceManager();
builder.Services.ConfigureAWSConfiguration(builder.Configuration);
builder.Services.ConfigureAWS(builder.Configuration);
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});
builder.Services.AddScoped<ValidationFilterAttribute>();
builder.Services.AddScoped<ValidationFilterPasswordConfirmation>();
builder.Services.AddAuthorization();
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();
builder.Services.ConfigureOptions<JwtBearerConfigurationOptions>();

builder.Services.AddControllers();


var app = builder.Build();
var logger = app.Services.GetRequiredService<ILoggerManager>();
app.ConfigureExceptionHandler(logger);

app.UseHttpsRedirection();
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.All
});
app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();



// Load configuration for AWS
builder.Services.Configure<AWSConfiguration>(builder.Configuration.GetSection("AWS"));

// Register services
builder.Services.AddSingleton<S3Service>();

// Register other services
// Example: builder.Services.AddSingleton<YourOtherService>();

// Other existing service registrations
builder.Services.AddControllers();
// Add any other services you need here...


// Configure middlewares
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();


