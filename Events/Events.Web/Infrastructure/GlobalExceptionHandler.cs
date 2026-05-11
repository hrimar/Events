using Microsoft.AspNetCore.Diagnostics;
namespace Events.Web.Infrastructure;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Unhandled exception occurred. Path: {Path}", httpContext.Request.Path);

        return ValueTask.FromResult(false); // Let UseExceptionHandler("/Home/Error") render the error page
    }
}
