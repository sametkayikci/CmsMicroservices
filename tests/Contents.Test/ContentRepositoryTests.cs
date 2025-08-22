namespace Contents.Test;

public class ContentRepositoryTests
{
    private static AppDbContext InMemoryDb()
    {
        var opt = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        return new AppDbContext(opt);
    }

    [Fact]
    public async Task Given_multiple_contents_When_getbyuser_Then_returns_filtered()
    {
        await using var db = InMemoryDb();
        var repo = new ContentRepository(db);
        var u1 = Guid.NewGuid();
        var u2 = Guid.NewGuid();

        await repo.AddAsync(
            new Content { Id = Guid.NewGuid(), UserId = u1, Title = "t1", Body = "b1", CreatedAtUtc = DateTime.UtcNow },
            CancellationToken.None);
        await repo.AddAsync(
            new Content { Id = Guid.NewGuid(), UserId = u2, Title = "t2", Body = "b2", CreatedAtUtc = DateTime.UtcNow },
            CancellationToken.None);

        var list = await repo.GetByUserAsync(u1, CancellationToken.None);
        list.Should().HaveCount(1);
        list[0].UserId.Should().Be(u1);
    }
}