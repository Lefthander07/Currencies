namespace Fuse8.BackendInternship.InternalApi.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            _logger.LogInformation("Входящий запрос: {Method} {Url} Query {Query}",
                httpContext.Request.Method,
                httpContext.Request.Path,
                httpContext.Request.QueryString);

            await _next(httpContext);
            _logger.LogInformation("Статус ответа: {StatusCode}", httpContext.Response.StatusCode);
        }
    }
}
