using EduCore.Shared.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace EduCore.Web.CustomMiddlewares
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;

        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
                await HandleNotFoundResource(httpContext);

            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Something went wrong");
               
                  var problemDetails = new ProblemDetails
                    {
                        Title = "Error While processing Http Request",
                        Detail = exception.Message,
                        Instance= httpContext.Request.Path,
                        Status= exception switch
                        {
                            NotFoundException => StatusCodes.Status404NotFound,
                            BadHttpRequestException => StatusCodes.Status400BadRequest,
                            _ => StatusCodes.Status500InternalServerError
                        }

                  };
                
                httpContext.Response.StatusCode = problemDetails.Status!.Value;
                httpContext.Response.ContentType = "application/json";
                await httpContext.Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
            }
        }

        private static async Task HandleNotFoundResource(HttpContext httpContext)
        {
            if (httpContext.Response.StatusCode == StatusCodes.Status404NotFound)
            {
                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Error While Processing The Http Request - End Point Not Found",
                    Detail = "The requested resource was not found."
                };
                httpContext.Response.ContentType = "application/json";
                await httpContext.Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
            }
        }
    }
}
