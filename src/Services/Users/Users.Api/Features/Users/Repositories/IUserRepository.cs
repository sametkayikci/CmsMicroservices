using Users.Api.Features.Users.Entities;


namespace Users.Api.Features.Users.Repositories;

public interface IUserRepository
{
    Task<List<User>> GetAllAsync(CancellationToken ct);
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<User> AddAsync(User user, CancellationToken ct);
    Task<User> UpdateAsync(User user, CancellationToken ct);
    Task DeleteAsync(User user, CancellationToken ct);
}