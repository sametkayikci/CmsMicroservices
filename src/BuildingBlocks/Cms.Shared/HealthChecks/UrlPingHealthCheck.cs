using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Cms.Shared.HealthChecks;

public sealed class UrlPingHealthCheck(IHttpClientFactory httpClientFactory, string url, TimeSpan? timeout = null)
    : IHealthCheck
{
    private readonly TimeSpan _timeout = timeout ?? TimeSpan.FromSeconds(2);

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken ct = default)
    {
        try
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(_timeout);

            var client = httpClientFactory.CreateClient("health");
            using var resp = await client.GetAsync(url, cts.Token);

            return resp.IsSuccessStatusCode
                ? HealthCheckResult.Healthy()
                : HealthCheckResult.Unhealthy($"StatusCode={(int)resp.StatusCode}");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(ex.Message, ex);
        }
    }
}