namespace Fuse8.BackendInternship.PublicApi
{
    public class RequestLogging
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLogging> _logger;

        public RequestLogging(RequestDelegate next, ILogger<RequestLogging> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            _logger.LogInformation("Incoming Request: {Method} {Url}",
                httpContext.Request.Method,
                httpContext.Request.Path);

            foreach (var param in httpContext.Request.Query)
            {
                _logger.LogInformation("Query Parameter: {Key} = {Value}", param.Key, param.Value);
            }

            await _next(httpContext);
        }
    }
}
