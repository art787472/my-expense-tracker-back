
using System.Text.Json;
using System;

namespace 記帳程式後端.MiddleWares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 500;

                var response = new { message = ex.Message, statusCode = 500 };
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }

        }
    }
}
