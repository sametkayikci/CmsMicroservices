namespace Users.Tests;

public class UserRepositoryTests
{
    private static AppDbContext InMemoryDb()
    {
        var opt = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        return new AppDbContext(opt);
    }

    [Fact]
    public async Task Given_user_persisted_When_getbyid_Then_returns_entity()
    {
        await using var db = InMemoryDb();
        var repo = new UserRepository(db);
        var u = new User { Id = Guid.NewGuid(), Email = "a@b.com", FullName = "Alice", CreatedAtUtc = DateTime.UtcNow };

        await repo.AddAsync(u, CancellationToken.None);
        var found = await repo.GetByIdAsync(u.Id, CancellationToken.None);

        found!.Email.Should().Be("a@b.com");
    }
}