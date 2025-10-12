
using System.Text.Json;
using System;
using Serilog;

namespace ExpenseTracker.MiddleWares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
                _logger.LogError(ex, "未處理的例外發生，Path: {Path}", context.Request.Path);

                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("伺服器發生未處理的錯誤。");
            }
            

        }
    }
}
