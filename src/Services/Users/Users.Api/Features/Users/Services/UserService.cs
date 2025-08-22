using Users.Api.Features.Users.Entities;


namespace Users.Api.Features.Users.Services;

public sealed class UserService(
    IUserRepository repo,
    IDateTime clock,
    ICacheService cache,
    IConfiguration cfg,
    IContentsClient contentsClient) : IUserService
{
    private static UserDto ToDto(User u) => new(u.Id, u.Email, u.FullName, u.CreatedAtUtc);


    public async Task<IReadOnlyList<UserDto>> GetAllAsync(CancellationToken ct)
    {
        const string key = "users:all";
        var cached = await cache.GetAsync<List<UserDto>>(key, ct);
        if (cached is not null) return cached;
        var list = (await repo.GetAllAsync(ct)).Select(ToDto).ToList();
        await cache.SetAsync(key, list, TimeSpan.FromMinutes(2), ct);
        return list;
    }
    
    public async Task<UserDto?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var key = $"users:id:{id}";
        var cached = await cache.GetAsync<UserDto>(key, ct);
        if (cached is not null) return cached;
        var entity = await repo.GetByIdAsync(id, ct);
        if (entity is null) return null;
        var dto = ToDto(entity);
        await cache.SetAsync(key, dto, TimeSpan.FromMinutes(5), ct);
        return dto;
    }

    public async Task<UserDto> CreateAsync(CreateUserRequest req, CancellationToken ct)
    {
        var entity = new User
        {
            Id = Guid.NewGuid(), Email = req.Email.Trim(), FullName = req.FullName.Trim(), CreatedAtUtc = clock.UtcNow
        };
        entity = await repo.AddAsync(entity, ct);
        await InvalidateAsync(entity.Id, ct);
        return ToDto(entity);
    }

    public async Task<UserDto?> UpdateAsync(Guid id, UpdateUserRequest req, CancellationToken ct)
    {
        var entity = await repo.GetByIdAsync(id, ct);
        if (entity is null) return null;
        entity.Email = req.Email.Trim();
        entity.FullName = req.FullName.Trim();
        entity = await repo.UpdateAsync(entity, ct);
        await InvalidateAsync(entity.Id, ct);
        return ToDto(entity);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
    {
        var entity = await repo.GetByIdAsync(id, ct);
        if (entity is null) return false;
        await repo.DeleteAsync(entity, ct);
        await InvalidateAsync(id, ct);
        // İçerikler temizlensin (Refit ile Contents API)
        var internalKey = cfg["Internal:Key"]!;
        await contentsClient.DeleteByUserAsync(id, internalKey, ct);
        return true;
    }


    private async Task InvalidateAsync(Guid id, CancellationToken ct)
    {
        await cache.RemoveAsync("users:all", ct);
        await cache.RemoveAsync($"users:id:{id}", ct);
    }
}