using System.Net;
using System.Text.Json;
using UserManagementAPI.Domain.Exceptions;

namespace UserManagementAPI.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = new
            {
                message = exception.Message,
                timestamp = DateTime.UtcNow,
                path = context.Request.Path
            };

            switch (exception)
            {
                case DomainException:
                    _logger.LogWarning(exception, "Domain exception occurred");
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response = new { message = exception.Message, timestamp = DateTime.UtcNow, path = context.Request.Path };
                    break;

                case UnauthorizedAccessException:
                    _logger.LogWarning(exception, "Unauthorized access attempted");
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    response = new { message = "Unauthorized access", timestamp = DateTime.UtcNow, path = context.Request.Path };
                    break;

                case ArgumentException:
                    _logger.LogWarning(exception, "Invalid argument provided");
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response = new { message = exception.Message, timestamp = DateTime.UtcNow, path = context.Request.Path };
                    break;

                default:
                    _logger.LogError(exception, "Unexpected error occurred");
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    response = new { message = "An unexpected error occurred", timestamp = DateTime.UtcNow, path = context.Request.Path };
                    break;
            }

            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
