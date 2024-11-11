using Microsoft.AspNetCore.HttpOverrides;
using NLog;
using Unit.API.Extensions;
using Unit.Service.Contracts;

var builder = WebApplication.CreateBuilder(args);

LogManager.Setup().LoadConfigurationFromFile(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));

builder.Services.ConfigureCors();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.ConfigureLogger();
builder.Services.ConfigureServiceManager();
builder.Services.ConfigureAWSConfiguration(builder.Configuration);
builder.Services.ConfigureAWS(builder.Configuration);
builder.Services.AddControllers();


var app = builder.Build();
var logger = app.Services.GetRequiredService<ILoggerManager>();
app.ConfigureExceptionHandler(logger);

//app.UseHttpsRedirection();
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.All
});
app.UseCors("CorsPolicy");
app.UseAuthorization();
app.MapControllers();


app.Run();

