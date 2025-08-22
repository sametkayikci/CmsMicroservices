namespace Contents.Api.Features.Contents.Repositories;


public interface IContentRepository
{
    Task<List<Content>> GetAllAsync(CancellationToken ct);
    Task<Content?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<List<Content>> GetByUserAsync(Guid userId, CancellationToken ct);
    Task<Content> AddAsync(Content entity, CancellationToken ct);
    Task<Content> UpdateAsync(Content entity, CancellationToken ct);
    Task DeleteAsync(Content entity, CancellationToken ct);
    Task<int> DeleteByUserAsync(Guid userId, CancellationToken ct);
}