namespace Users.Api.Features.Users.Services;

public interface IUserService
{
    Task<IReadOnlyList<UserDto>> GetAllAsync(CancellationToken ct);
    Task<UserDto?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<UserDto> CreateAsync(CreateUserRequest req, CancellationToken ct);
    Task<UserDto?> UpdateAsync(Guid id, UpdateUserRequest req, CancellationToken ct);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct);
}