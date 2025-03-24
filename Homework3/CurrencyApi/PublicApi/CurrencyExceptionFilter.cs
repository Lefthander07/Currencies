using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

public class CurrencyExceptionFilter : IExceptionFilter
{
    private readonly ILogger<CurrencyExceptionFilter> _logger;

    public CurrencyExceptionFilter(ILogger<CurrencyExceptionFilter> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        if (context.Exception is ApiRequestLimitException)
        {
            _logger.LogError(context.Exception, "API Request limit exceeded.");
            context.Result = new ObjectResult("Too Many Requests")
            {
                StatusCode = StatusCodes.Status429TooManyRequests
            };
        }
        else if (context.Exception is CurrencyNotFoundException)
        {
            context.Result = new NotFoundResult();
        }
        else
        {
            _logger.LogError(context.Exception, "An unexpected error occured.");
            context.Result = new ObjectResult("Internal Server Error")
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }
        context.ExceptionHandled = true;
    }
};

public class ApiRequestLimitException : Exception
{
    public ApiRequestLimitException(string message) : base(message) { }
}

public class CurrencyNotFoundException : Exception
{
    public CurrencyNotFoundException(string message) : base(message) { }
}