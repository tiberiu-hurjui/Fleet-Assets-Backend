using Fleet_Assets_Backend.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Fleet_Assets_Backend.Api.Middleware;

public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger = logger;

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Bad request: {Message}", ex.Message);
            await WriteProblem(context, StatusCodes.Status400BadRequest, "Bad Request", ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation: {Message}", ex.Message);
            await WriteProblem(context, StatusCodes.Status409Conflict, "Conflict", ex.Message);
        }

        catch (VehicleNotFoundException ex)
        {
            _logger.LogWarning(ex, "Vehicle not found. VehicleId={VehicleId}", ex.VehicleId);
            await WriteProblem(context, StatusCodes.Status404NotFound, "Not Found", "Vehicle not found.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await WriteProblem(context, StatusCodes.Status500InternalServerError, "Internal Server Error",
                "Something went wrong.");
        }
    }

    private static async Task WriteProblem(HttpContext context, int statusCode, string title, string detail)
    {
        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = statusCode;

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };

        if (context.Response.Headers.TryGetValue("X-Correlation-Id", out var cid) && cid.Count > 0)
        {
            problem.Extensions["correlationId"] = cid.ToString();
        }

        var json = JsonSerializer.Serialize(problem);
        await context.Response.WriteAsync(json);
    }
}
