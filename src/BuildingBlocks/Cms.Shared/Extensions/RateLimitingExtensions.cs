namespace Cms.Shared.Extensions;

public static class RateLimitingExtensions
{
    public static IServiceCollection AddFixedWindowRateLimiting(this IServiceCollection services, int permitLimit = 100,
        int windowSeconds = 60)
    {
        services.AddRateLimiter(o =>
        {
            o.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(ctx =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: ctx.Connection.RemoteIpAddress?.ToString() ?? "anon",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = permitLimit,
                        Window = TimeSpan.FromSeconds(windowSeconds),
                        QueueLimit = 0,
                        AutoReplenishment = true
                    }));
            o.RejectionStatusCode = 429;
        });
        return services;
    }
}