using Amazon.Runtime;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using Unit.Entities.ErrorModel;
using Unit.Entities.Exceptions;
using Unit.Entities.Models;
using Unit.Service.Contracts;

namespace Unit.API.Extensions
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureExceptionHandler(this WebApplication app,
       ILoggerManager logger)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        logger.LogError($"Something went wrong: {contextFeature.Error}");

                        var message = "Internal Server Error.";

                        switch (contextFeature.Error)
                        {
                            case NotFoundException ex:
                                context.Response.StatusCode = StatusCodes.Status404NotFound;
                                message = ex.Message;
                                break;

                            case BadRequestException ex:
                                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                                message = ex.Message;
                                break;

                            case AmazonServiceException ex:
                                context.Response.StatusCode = (int)ex.StatusCode;
                                message = ex.Message;
                                break;

                            default:
                                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                                break;
                        }

                        await context.Response.WriteAsync(new ErrorDetails()
                        {
                            StatusCode = context.Response.StatusCode,
                            Message = message
                        }.ToString());
                    }
                });
            });
        }
    }
}
