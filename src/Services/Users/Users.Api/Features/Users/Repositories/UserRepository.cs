using Users.Api.Features.Users.Entities;


namespace Users.Api.Features.Users.Repositories;

public sealed class UserRepository(AppDbContext db) : IUserRepository
{
    public Task<List<User>> GetAllAsync(CancellationToken ct) =>
        db.Users.AsNoTracking().OrderBy(x => x.FullName).ToListAsync(ct);

    public Task<User?> GetByIdAsync(Guid id, CancellationToken ct) =>
        db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<User> AddAsync(User user, CancellationToken ct)
    {
        db.Users.Add(user);
        await db.SaveChangesAsync(ct);
        return user;
    }

    public async Task<User> UpdateAsync(User user, CancellationToken ct)
    {
        db.Users.Update(user);
        await db.SaveChangesAsync(ct);
        return user;
    }

    public async Task DeleteAsync(User user, CancellationToken ct)
    {
        db.Users.Remove(user);
        await db.SaveChangesAsync(ct);
    }
}