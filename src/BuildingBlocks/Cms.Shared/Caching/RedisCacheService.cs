namespace Cms.Shared.Caching;

public sealed class RedisCacheService(IDistributedCache cache) : ICacheService
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        var bytes = await cache.GetAsync(key, ct);
        return bytes is null ? default : JsonSerializer.Deserialize<T>(bytes, Json);
    }


    public async Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken ct = default)
    {
        var bytes = JsonSerializer.SerializeToUtf8Bytes(value, Json);
        await cache.SetAsync(key, bytes, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = ttl
        }, ct);
    }


    public Task RemoveAsync(string key, CancellationToken ct = default)
        => cache.RemoveAsync(key, ct);
}