namespace Users.Tests;

public class RedisCacheServiceTests
{
    [Fact]
    public async Task Given_memory_cache_When_set_get_remove_Then_roundtrip_succeeds()
    {
        var services = new ServiceCollection();
        services.AddDistributedMemoryCache();
        var sp = services.BuildServiceProvider();

        var distributed = sp.GetRequiredService<IDistributedCache>();
        var cache = new RedisCacheService(distributed);

        var key = $"k:{Guid.NewGuid()}";
        (await cache.GetAsync<string>(key)).Should().BeNull();

        await cache.SetAsync(key, "value", TimeSpan.FromSeconds(60));
        (await cache.GetAsync<string>(key)).Should().Be("value");

        await cache.RemoveAsync(key);
        (await cache.GetAsync<string>(key)).Should().BeNull();
    }
}