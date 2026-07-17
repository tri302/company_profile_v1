using System.Net;
using System.Text.Json;

namespace company_profile.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public GlobalExceptionMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger,
            IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Unhandled Exception Occurred");

                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(
            HttpContext context,
            Exception exception)
        {
            context.Response.ContentType = "text/html";

            var statusCode = exception switch
            {
                UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                KeyNotFoundException => StatusCodes.Status404NotFound,
                ArgumentException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            };

            context.Response.StatusCode = statusCode;

            if (context.Request.Path.StartsWithSegments("/api"))
            {
                context.Response.ContentType = "application/json";

                var response = new
                {
                    Success = false,
                    Message = _env.IsDevelopment()
                        ? exception.Message
                        : "Internal Server Error",
                    StatusCode = statusCode
                };

                await context.Response.WriteAsync(
                    JsonSerializer.Serialize(response));

                return;
            }
            // Jika bukan API (Web), Redirect ke ErrorController
            // Kita arahkan berdasarkan status code agar ErrorController bisa menentukan view-nya
            context.Response.Redirect($"/Error/{statusCode}");
        }
    }
}
