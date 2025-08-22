namespace Contents.Api.Features.Contents.Services;

public sealed class ContentService(
    IContentRepository repo,
    IDateTime clock,
    ICacheService cache,
    IUsersClient usersClient,
    IConfigurationRoot cfg) : IContentService
{
    private static ContentDto ToDto(Content c) => new(c.Id, c.UserId, c.Title, c.Body, c.CreatedAtUtc);

    public async Task<IReadOnlyList<ContentDto>> GetAllAsync(CancellationToken ct)
    {
        const string key = "contents:all";
        var cached = await cache.GetAsync<List<ContentDto>>(key, ct);
        if (cached is not null) return cached;
        var list = (await repo.GetAllAsync(ct)).Select(ToDto).ToList();
        await cache.SetAsync(key, list, TimeSpan.FromMinutes(2), ct);
        return list;
    }

    public async Task<ContentDto?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var key = $"contents:id:{id}";
        var cached = await cache.GetAsync<ContentDto>(key, ct);
        if (cached is not null) return cached;
        var content = await repo.GetByIdAsync(id, ct);
        if (content is null) return null;
        var dto = ToDto(content);
        await cache.SetAsync(key, dto, TimeSpan.FromMinutes(5), ct);
        return dto;
    }

    public async Task<IReadOnlyList<ContentDto>> GetByUserAsync(Guid userId, CancellationToken ct)
    {
        var key = $"contents:user:{userId}";
        var cached = await cache.GetAsync<List<ContentDto>>(key, ct);
        if (cached is not null) return cached;
        var list = (await repo.GetByUserAsync(userId, ct)).Select(ToDto).ToList();
        await cache.SetAsync(key, list, TimeSpan.FromMinutes(2), ct);
        return list;
    }

    public async Task<ContentDto?> CreateAsync(CreateContentRequest req, CancellationToken ct)
    {
        // Kullanıcı validasyonu (Refit -> Users API)
        _ = await usersClient.GetByIdAsync(req.UserId, ct);

        var content = new Content
        {
            Id = Guid.NewGuid(), UserId = req.UserId, Title = req.Title.Trim(), Body = req.Body.Trim(),
            CreatedAtUtc = clock.UtcNow
        };
        content = await repo.AddAsync(content, ct);
        await InvalidateAsync(content, ct);
        return ToDto(content);
    }

    public async Task<ContentDto?> UpdateAsync(Guid id, UpdateContentRequest req, CancellationToken ct)
    {
        var e = await repo.GetByIdAsync(id, ct);
        if (e is null) return null;
        e.Title = req.Title.Trim();
        e.Body = req.Body.Trim();
        e = await repo.UpdateAsync(e, ct);
        await InvalidateAsync(e, ct);
        return ToDto(e);
    }


    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
    {
        var e = await repo.GetByIdAsync(id, ct);
        if (e is null) return false;
        await repo.DeleteAsync(e, ct);
        await InvalidateAsync(e, ct);
        return true;
    }


    public async Task<int> DeleteByUserAsync(Guid userId, CancellationToken ct)
    {
        var affected = await repo.DeleteByUserAsync(userId, ct);
        await cache.RemoveAsync("contents:all", ct);
        await cache.RemoveAsync($"contents:user:{userId}", ct);
        return affected;
    }

    public async Task<bool> UpdateUserViaUsersApiAsync(Guid userId, UpdateUserRequest req, CancellationToken ct)
    {
        var updated = await usersClient.UpdateAsync(userId, req, ct);
        await cache.RemoveAsync("contents:all", ct);
        await cache.RemoveAsync($"contents:user:{userId}", ct);
        return updated is not null;
    }
    
    private async Task InvalidateAsync(Content e, CancellationToken ct)
    {
        await cache.RemoveAsync("contents:all", ct);
        await cache.RemoveAsync($"contents:id:{e.Id}", ct);
        await cache.RemoveAsync($"contents:user:{e.UserId}", ct);
    }
}