using System.IO;
using System.Text.Json;
using Cms.Shared.Middleware;
using Microsoft.Extensions.Logging;

namespace Users.Tests;

public class ExceptionHandlingMiddlewareTests
{
    [Fact]
    public async Task Given_throwing_delegate_When_invoke_Then_returns_500_json()
    {
        var logger = new Mock<ILogger<ExceptionHandlingMiddleware>>();
        var mw = new ExceptionHandlingMiddleware(_ => throw new InvalidOperationException("boom"), logger.Object);
        var ctx = new DefaultHttpContext
        {
            Response =
            {
                Body = new MemoryStream()
            }
        };

        await mw.Invoke(ctx);

        Assert.Equal(500, ctx.Response.StatusCode);
        ctx.Response.Body.Position = 0;
        using var r = new StreamReader(ctx.Response.Body);
        var text = await r.ReadToEndAsync();
        using var doc = JsonDocument.Parse(text);
        Assert.True(doc.RootElement.TryGetProperty("message", out _));
    }
}