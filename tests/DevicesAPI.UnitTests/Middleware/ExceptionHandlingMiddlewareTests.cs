using DevicesAPI.API.Middleware;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;

namespace DevicesAPI.UnitTests.Middleware;

public class ExceptionHandlingMiddlewareTests
{
	private static ExceptionHandlingMiddleware CreateMiddleware(RequestDelegate next) =>
		new(next, NullLogger<ExceptionHandlingMiddleware>.Instance);

	private static DefaultHttpContext CreateContext()
	{
		var context = new DefaultHttpContext();
		context.Response.Body = new MemoryStream();
		return context;
	}

	[Fact]
	public async Task Invoke_ShouldReturn404_WhenKeyNotFoundExceptionThrown()
	{
		var middleware = CreateMiddleware(_ => throw new KeyNotFoundException("not found"));
		var context = CreateContext();

		await middleware.InvokeAsync(context);

		context.Response.StatusCode.Should().Be(StatusCodes.Status404NotFound);
		context.Response.ContentType.Should().Be("application/problem+json");
	}

	[Fact]
	public async Task Invoke_ShouldReturn409_WhenInvalidOperationExceptionThrown()
	{
		var middleware = CreateMiddleware(_ => throw new InvalidOperationException("conflict"));
		var context = CreateContext();

		await middleware.InvokeAsync(context);

		context.Response.StatusCode.Should().Be(StatusCodes.Status409Conflict);
		context.Response.ContentType.Should().Be("application/problem+json");
	}

	[Fact]
	public async Task Invoke_ShouldReturn500_WhenUnhandledExceptionThrown()
	{
		var middleware = CreateMiddleware(_ => throw new Exception("boom"));
		var context = CreateContext();

		await middleware.InvokeAsync(context);

		context.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
	}

	[Fact]
	public async Task Invoke_ShouldCallNext_WhenNoExceptionThrown()
	{
		var wasCalled = false;
		var middleware = CreateMiddleware(_ => { wasCalled = true; return Task.CompletedTask; });
		var context = CreateContext();

		await middleware.InvokeAsync(context);

		wasCalled.Should().BeTrue();
		context.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
	}
}