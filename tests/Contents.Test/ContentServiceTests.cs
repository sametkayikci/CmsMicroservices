namespace Contents.Test;

public class ContentServiceTests
{
    private static AppDbContext InMemoryDb()
    {
        var opt = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        return new AppDbContext(opt);
    }

    [Fact]
    public async Task Given_valid_user_When_create_content_Then_succeeds_and_validates_user()
    {
        await using var db = InMemoryDb();
        var repo = new ContentRepository(db);
        var cache = new Mock<ICacheService>();
        var clock = new Mock<IDateTime>();
        clock.Setup(x => x.UtcNow).Returns(DateTime.UtcNow);

        var usersClient = new Mock<IUsersClient>();
        usersClient.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UserDto(Guid.NewGuid(), "e@x.com", "Eve", DateTime.UtcNow));

        var cfg = new ConfigurationBuilder().Build();
        var sut = new ContentService(repo, clock.Object, cache.Object, usersClient.Object, cfg);

        var dto = await sut.CreateAsync(new CreateContentRequest(Guid.NewGuid(), "T", "B"), CancellationToken.None);
        dto!.Title.Should().Be("T");
        usersClient.Verify(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
        (await db.Contents.CountAsync()).Should().Be(1);
    }

    [Fact]
    public async Task Given_cache_When_getbyid_twice_Then_repo_called_once()
    {
        var id = Guid.NewGuid();
        var repo = new Mock<IContentRepository>();
        repo.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Content
                { Id = id, UserId = Guid.NewGuid(), Title = "t", Body = "b", CreatedAtUtc = DateTime.UtcNow });

        var cache = new Mock<ICacheService>();
        var dto = new ContentDto(id, Guid.NewGuid(), "t", "b", DateTime.UtcNow);
        cache.SetupSequence(c => c.GetAsync<ContentDto>($"contents:id:{id}", It.IsAny<CancellationToken>()))
            .ReturnsAsync((ContentDto?)null)
            .ReturnsAsync(dto);

        var clock = Mock.Of<IDateTime>(x => x.UtcNow == DateTime.UtcNow);
        var usersClient = new Mock<IUsersClient>();
        var cfg = new ConfigurationBuilder().Build();
        var sut = new ContentService(repo.Object, clock, cache.Object, usersClient.Object, cfg);

        _ = await sut.GetByIdAsync(id, CancellationToken.None);
        var two = await sut.GetByIdAsync(id, CancellationToken.None);
        two.Should().BeSameAs(dto);
        repo.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Given_userid_When_delete_by_user_Then_cache_invalidated_and_count_returned()
    {
        await using var db = InMemoryDb();
        var repo = new ContentRepository(db);
        var cache = new Mock<ICacheService>();
        var clock = Mock.Of<IDateTime>(x => x.UtcNow == DateTime.UtcNow);
        var usersClient = new Mock<IUsersClient>();
        var cfg = new ConfigurationBuilder().Build();
        var sut = new ContentService(repo, clock, cache.Object, usersClient.Object, cfg);

        var uid = Guid.NewGuid();
        await repo.AddAsync(
            new Content
            {
                Id = Guid.NewGuid(), UserId = uid, Title = "t1", Body = "b1", CreatedAtUtc = DateTime.UtcNow
            }, CancellationToken.None);
        await repo.AddAsync(
            new Content
            {
                Id = Guid.NewGuid(), UserId = uid, Title = "t2", Body = "b2", CreatedAtUtc = DateTime.UtcNow
            }, CancellationToken.None);

        var affected = await sut.DeleteByUserAsync(uid, CancellationToken.None);
        affected.Should().Be(2);
        cache.Verify(c => c.RemoveAsync("contents:all", It.IsAny<CancellationToken>()), Times.Once);
        cache.Verify(c => c.RemoveAsync($"contents:user:{uid}", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Given_update_request_When_forward_to_users_api_Then_returns_true()
    {
        var repo = new Mock<IContentRepository>();
        var cache = new Mock<ICacheService>();
        var clock = Mock.Of<IDateTime>(x => x.UtcNow == DateTime.UtcNow);
        var usersClient = new Mock<IUsersClient>();
        usersClient.Setup(u =>
                u.UpdateAsync(It.IsAny<Guid>(), It.IsAny<UpdateUserRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UserDto(Guid.NewGuid(), "e@x.com", "Eve", DateTime.UtcNow));
        var cfg = new ConfigurationBuilder().Build();
        var sut = new ContentService(repo.Object, clock, cache.Object, usersClient.Object, cfg);

        var ok = await sut.UpdateUserViaUsersApiAsync(Guid.NewGuid(), new UpdateUserRequest("e@x.com", "Eve"),
            CancellationToken.None);
        ok.Should().BeTrue();
        usersClient.VerifyAll();
    }
}