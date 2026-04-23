using System.Net;
using System.Text.Json;

namespace DevicesAPI.API.Middleware;

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
		catch (KeyNotFoundException ex)
		{
			_logger.LogWarning(ex, "Resource not found");
			await WriteProblemAsync(context, HttpStatusCode.NotFound, "Not Found", ex.Message);
		}
		catch (InvalidOperationException ex)
		{
			_logger.LogWarning(ex, "Business rule violation");
			await WriteProblemAsync(context, HttpStatusCode.Conflict, "Conflict", ex.Message);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Unhandled exception");
			await WriteProblemAsync(context, HttpStatusCode.InternalServerError, "Internal Server Error", "An unexpected error occurred.");
		}
	}

	private static async Task WriteProblemAsync(HttpContext context, HttpStatusCode statusCode, string title, string detail)
	{
		context.Response.StatusCode = (int)statusCode;
		context.Response.ContentType = "application/problem+json";

		var problem = new
		{
			type = $"https://httpstatuses.io/{(int)statusCode}",
			title,
			status = (int)statusCode,
			detail
		};

		await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
	}
}