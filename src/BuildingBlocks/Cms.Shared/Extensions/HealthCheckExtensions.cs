using Cms.Shared.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Cms.Shared.Extensions;

public static class HealthCheckExtensions
{
    /// <summary>
    /// Sadece API'ler ayakta mı? (Users & Contents)
    /// runningInsideDocker = true → Docker ağı üzerinden çağırır (users.api:8080, contents.api:8080)
    /// runningInsideDocker = false → Host üzerinden çağırır (localhost:5001/5002)
    /// </summary>
    public static IServiceCollection AddApiLivenessChecks(this IServiceCollection services, bool runningInsideDocker)
    {
        services.AddHttpClient();

        var usersUrl    = runningInsideDocker ? "http://users.api:8080/health"    : "http://localhost:5001/health";
        var contentsUrl = runningInsideDocker ? "http://contents.api:8080/health" : "http://localhost:5002/health";

        var hc = services.AddHealthChecks();

        hc.AddTypeActivatedCheck<UrlPingHealthCheck>(
            name: "users-api",
            failureStatus: HealthStatus.Unhealthy,
            tags: ["api"],
            args: [usersUrl, TimeSpan.FromSeconds(2)]);

        hc.AddTypeActivatedCheck<UrlPingHealthCheck>(
            name: "contents-api",
            failureStatus: HealthStatus.Unhealthy,
            tags: ["api"],
            args: [contentsUrl, TimeSpan.FromSeconds(2)]);

        return services;
    }
}