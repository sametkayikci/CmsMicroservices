namespace Cms.Shared.RefitClients;

public interface IUsersClient
{
    [Get("/users/{id}")]
    Task<UserDto> GetByIdAsync(Guid id, CancellationToken ct = default);

    [Put("/users/{id}")]
    Task<UserDto> UpdateAsync(Guid id, [Body] UpdateUserRequest request, CancellationToken ct = default);
}