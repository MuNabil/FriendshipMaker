namespace API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    // To pass request to the next middleware 
    //iff any middleware throw an exception 
    //then the response will go back to this middleware to handle this exception.

    private readonly ILogger<ExceptionMiddleware> _logger;
    // To log the exception detail into terminal.

    private readonly IHostEnvironment _env;
    // To check whether environment we'r in to send the ex detail based on environment.

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
    {
        _env = env;
        _logger = logger;
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
            _logger.LogError(ex, ex.Message);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = _env.IsDevelopment() ?
                new ApiException(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString()) :
                new ApiException(context.Response.StatusCode, "Internal server error");

            // To send the response back in camelCase
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(response, options);

            await context.Response.WriteAsync(json);
        }
    }
}
