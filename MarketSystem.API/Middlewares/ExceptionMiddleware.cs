using Application.Exceptions;

namespace MarketSystem.API.Middlewares;

public class ExceptionMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger)
    {
        _logger = logger;
    }
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            
            context.Response.ContentType = "application/json";
            
            if (e is BaseException baseException)
            {
                context.Response.StatusCode = baseException.StatusCode;
                
                context.Response.Clear();
                
                await context.Response.WriteAsJsonAsync(new
                {
                    statusCode = baseException.StatusCode,
                    message = baseException.Message
                });
            }
            
            else
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError; 
                
                context.Response.Clear();
                
                await context.Response.WriteAsJsonAsync(new
                {
                    message = "Internal server error"
                });
            }
            
        }
    }
}