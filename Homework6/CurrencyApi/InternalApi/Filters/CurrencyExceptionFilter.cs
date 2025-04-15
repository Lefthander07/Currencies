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
                setResponse("Request limit reached.,", StatusCodes.Status429TooManyRequests);
                break;

            case CurrencyNotFoundException ex:
                _logger.LogWarning(ex, "Currency not found");
                setResponse(ex.Message, StatusCodes.Status404NotFound);
                break;
            default:
                _logger.LogError(context.Exception, context.Exception.Message);
                setResponse(context.Exception.Message, StatusCodes.Status500InternalServerError);
                break;
        }

        context.ExceptionHandled = true;

        void setResponse(string errorDescription, int httpStatusCode)
        {
            context.Result = new JsonResult(new ProblemDetails
            {
                Title = errorDescription,
                Status = httpStatusCode
            });
            context.HttpContext.Response.StatusCode = httpStatusCode;
        }
    }
}