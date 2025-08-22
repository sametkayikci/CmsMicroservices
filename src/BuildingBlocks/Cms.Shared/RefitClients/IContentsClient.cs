namespace Cms.Shared.RefitClients;

public interface IContentsClient
{
    [Delete("/internal/contents/by-user/{userId}")]
    Task DeleteByUserAsync(Guid userId, [Header("X-Internal-Key")] string internalKey, CancellationToken ct = default);
}