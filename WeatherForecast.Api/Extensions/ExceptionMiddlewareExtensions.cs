using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Net;

namespace WeatherForecast.Api.Extensions
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.ContentType = "application/json";

                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        var statusCode = (int)HttpStatusCode.InternalServerError;

                        // Log detalhado com Serilog
                        Log.Error(contextFeature.Error, "Unhandled exception occurred");

                        var problemDetails = new ProblemDetails
                        {
                            Status = statusCode,
                            Title = "Erro Interno no Servidor",
                            Detail = contextFeature.Error.Message,
                            Instance = context.Request.Path
                        };

                        context.Response.StatusCode = statusCode;
                        await context.Response.WriteAsJsonAsync(problemDetails);
                    }
                });
            });
        }
    }
}