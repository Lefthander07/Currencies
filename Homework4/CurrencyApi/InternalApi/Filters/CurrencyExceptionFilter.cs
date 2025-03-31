using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Fuse8.BackendInternship.InternalApi.Exceptions;
public class CurrencyExceptionFilter : IExceptionFilter
{
    private readonly ILogger<CurrencyExceptionFilter> _logger;

    public CurrencyExceptionFilter(ILogger<CurrencyExceptionFilter> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        switch (context.Exception)
        {
            case ApiRequestLimitException ex:
                _logger.LogError(ex, "API Request limit exceeded");
                context.Result = new JsonResult(new ProblemDetails
                {
                    Title = "Request limit reached.",
                    Status = StatusCodes.Status429TooManyRequests
                });
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                break;

            case CurrencyNotFoundException ex:
                _logger.LogWarning(ex, "Currency not found");
                context.Result = new JsonResult(new ProblemDetails
                {
                    Title = "Currency not found.",
                    Status = StatusCodes.Status404NotFound
                });
                context.HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                break;
            default:
                _logger.LogError(context.Exception, "An unexpected error occurred");
                context.Result = new JsonResult(new ProblemDetails
                {
                    Title = "An unexpected error occurred",
                    Status = StatusCodes.Status500InternalServerError
                });
                context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                break;
        }

        context.ExceptionHandled = true;
    }
}