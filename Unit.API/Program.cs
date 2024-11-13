using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using NLog;
using Unit.API.ActionFilter;
using Unit.API.Configuration;
using Unit.API.Extensions;
using Unit.Repository.Contracts;
using Unit.Service;
using Unit.Service.Contracts;
using Unit.Shared.DataTransferObjects;

var builder = WebApplication.CreateBuilder(args);

LogManager.Setup().LoadConfigurationFromFile(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));

builder.Services.ConfigureCors();

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.ConfigureLogger();

builder.Services.ConfigureServiceManager();

builder.Services.ConfigureRepositoryManager();

builder.Services.ConfigureAWSConfiguration(builder.Configuration);
builder.Services.ConfigureAWS(builder.Configuration);

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddScoped<ValidationFilterAttribute>();
builder.Services.AddScoped<ValidationFilterPasswordConfirmation>();
builder.Services.AddScoped<ImageFileValidationFilter>();

builder.Services.AddAuthorization();
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();
builder.Services.ConfigureOptions<JwtBearerConfigurationOptions>();

builder.Services.AddScoped<IDataShaper<UserDto>, DataShaper<UserDto>>();
builder.Services.AddScoped<IDataShaper<PostDto>, DataShaper<PostDto>>();

builder.Services.AddControllers();

// Build the app
var app = builder.Build();
var logger = app.Services.GetRequiredService<ILoggerManager>();
app.ConfigureExceptionHandler(logger);

//app.UseHttpsRedirection();
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.All
});
app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
