namespace Contents.Api.Features.Contents.Repositories;

public sealed class ContentRepository(AppDbContext db) : IContentRepository
{
    public Task<List<Content>> GetAllAsync(CancellationToken ct) =>
        db.Contents.AsNoTracking().OrderByDescending(x => x.CreatedAtUtc).ToListAsync(ct);

    public Task<Content?> GetByIdAsync(Guid id, CancellationToken ct) =>
        db.Contents.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task<List<Content>> GetByUserAsync(Guid userId, CancellationToken ct) => db.Contents.AsNoTracking()
        .Where(x => x.UserId == userId).OrderByDescending(x => x.CreatedAtUtc).ToListAsync(ct);

    public async Task<Content> AddAsync(Content e, CancellationToken ct)
    {
        db.Contents.Add(e);
        await db.SaveChangesAsync(ct);
        return e;
    }

    public async Task<Content> UpdateAsync(Content e, CancellationToken ct)
    {
        db.Contents.Update(e);
        await db.SaveChangesAsync(ct);
        return e;
    }

    public async Task DeleteAsync(Content e, CancellationToken ct)
    {
        db.Contents.Remove(e);
        await db.SaveChangesAsync(ct);
    }

    public async Task<int> DeleteByUserAsync(Guid userId, CancellationToken ct)
    {
        var q = db.Contents.Where(x => x.UserId == userId);
        db.Contents.RemoveRange(q);
        return await db.SaveChangesAsync(ct);
    }
}