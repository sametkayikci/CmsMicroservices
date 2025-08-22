namespace Contents.Api.Features.Contents.Services;


public interface IContentService
{
    Task<IReadOnlyList<ContentDto>> GetAllAsync(CancellationToken ct);
    Task<ContentDto?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<ContentDto>> GetByUserAsync(Guid userId, CancellationToken ct);
    Task<ContentDto?> CreateAsync(CreateContentRequest req, CancellationToken ct);
    Task<ContentDto?> UpdateAsync(Guid id, UpdateContentRequest req, CancellationToken ct);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct);
    Task<int> DeleteByUserAsync(Guid userId, CancellationToken ct);
    Task<bool> UpdateUserViaUsersApiAsync(Guid userId, UpdateUserRequest req, CancellationToken ct);
}