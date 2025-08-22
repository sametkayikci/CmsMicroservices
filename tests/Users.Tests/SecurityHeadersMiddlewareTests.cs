namespace Users.Tests;

public class SecurityHeadersMiddlewareTests
{
    [Fact]
    public async Task Given_request_When_invoke_Then_security_headers_are_set()
    {
        var mw = new SecurityHeadersMiddleware(_ => Task.CompletedTask);
        var ctx = new DefaultHttpContext();
        await mw.Invoke(ctx);

        Assert.True(ctx.Response.Headers.ContainsKey("X-Content-Type-Options"));
        Assert.True(ctx.Response.Headers.ContainsKey("X-Frame-Options"));
        Assert.True(ctx.Response.Headers.ContainsKey("Referrer-Policy"));
    }
}