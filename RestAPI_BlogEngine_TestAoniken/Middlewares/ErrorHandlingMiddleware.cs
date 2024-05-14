using RestAPi_BlogEngine_TestAoniken.Exceptions;
using System.Net;
using System.Text.Json;

namespace RestAPi_BlogEngine_TestAoniken.Middlewares
{
    // Middleware for globally handling exceptions in the application.
    public class ErrorHandlingMiddleware
    {
        // Delegate to the next middleware in the request pipeline.
        private readonly RequestDelegate _next;

        // Constructor that accepts the next middleware delegate.
        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        // Invoke method to intercept HTTP context.
        public async Task Invoke(HttpContext context)
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

        // Static method to handle exceptions.
        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            HttpStatusCode status;
            string message;


            var apiException = ex as ApiException;
            if (apiException != null)
            {
                status = (HttpStatusCode)apiException.StatusCode;
                message = apiException.Message;
            }
            else
            {
                status = HttpStatusCode.InternalServerError;
                message = "An unhandled exception occurred.";
            }

            var result = JsonSerializer.Serialize(new { error = message });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)status;
            return context.Response.WriteAsync(result);
        }
    }
}