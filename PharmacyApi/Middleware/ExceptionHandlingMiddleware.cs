using System.Net;
using System.Text.Json;
using PharmacyApi.Exceptions;

namespace PharmacyApi.Middleware;

/// <summary>
/// Central place to catch exceptions thrown anywhere in the request pipeline and translate
/// them into consistent JSON error responses, so controllers don't need try/catch blocks.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        HttpStatusCode statusCode;
        string message;

        if (exception is NotFoundException)
        {
            statusCode = HttpStatusCode.NotFound;
            message = exception.Message;
        }
        else if (exception is DomainValidationException)
        {
            statusCode = HttpStatusCode.BadRequest;
            message = exception.Message;
        }
        else
        {
            // Unexpected error: don't leak internal details to the client.
            statusCode = HttpStatusCode.InternalServerError;
            message = "An unexpected error occurred.";
        }

        if (statusCode == HttpStatusCode.InternalServerError)
        {
            _logger.LogError(exception, "Unhandled exception while processing {Method} {Path}", context.Request.Method, context.Request.Path);
        }
        else
        {
            // Expected client errors (400/404): log the reason only, no stack trace.
            _logger.LogWarning("Request {Method} {Path} failed with {StatusCode}: {Message}", context.Request.Method, context.Request.Path, (int)statusCode, message);
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var body = JsonSerializer.Serialize(new
        {
            status = (int)statusCode,
            message,
        });

        await context.Response.WriteAsync(body);
    }
}
