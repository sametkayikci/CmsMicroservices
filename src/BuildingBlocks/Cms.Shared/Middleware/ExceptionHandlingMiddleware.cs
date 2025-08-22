namespace Cms.Shared.Middleware;

public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> log)
{
    public async Task Invoke(HttpContext ctx)
    {
        try
        {
            await next(ctx);
        }
        catch (Exception ex)
        {
            log.LogError(ex, "Unhandled exception");
            ctx.Response.ContentType = "application/json";
            ctx.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            var payload = new
            {
                message = "Beklenmeyen bir hata oluştu.", 
                traceId = ctx.TraceIdentifier
            };
            await ctx.Response.WriteAsync(JsonSerializer.Serialize(payload));
        }
    }
}