namespace Users.Tests;

public class UserServiceTests
{
    private static AppDbContext InMemoryDb()
    {
        var opt = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        return new AppDbContext(opt);
    }

    [Fact]
    public async Task Given_valid_request_When_create_Then_persists_and_returns_dto()
    {
        await using var db = InMemoryDb();
        var repo = new UserRepository(db);
        var cache = new Mock<ICacheService>();
        var clock = new Mock<IDateTime>();
        clock.Setup(x => x.UtcNow).Returns(DateTime.UtcNow);
        var cfg = new ConfigurationBuilder().AddInMemoryCollection(
            new Dictionary<string, string?> { ["Internal:Key"] = "k" }).Build();
        var contentsClient = new Mock<IContentsClient>();
        var sut = new UserService(repo, clock.Object, cache.Object, cfg, contentsClient.Object);

        var dto = await sut.CreateAsync(new CreateUserRequest("a@b.com", "Alice"), CancellationToken.None);
        dto.Email.Should().Be("a@b.com");
        (await db.Users.CountAsync()).Should().Be(1);
    }


    [Fact]
    public async Task Given_cache_enabled_When_getall_twice_Then_repo_called_once_and_cache_hit_on_second()
    {
        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([
                new User { Id = Guid.NewGuid(), Email = "e@x.com", FullName = "Eve", CreatedAtUtc = DateTime.UtcNow }
            ]);

        var cache = new Mock<ICacheService>();
        var list = new List<UserDto> { new(Guid.NewGuid(), "e@x.com", "Eve", DateTime.UtcNow) };
        cache.SetupSequence(c => c.GetAsync<List<UserDto>>("users:all", It.IsAny<CancellationToken>()))
            .ReturnsAsync((List<UserDto>?)null)
            .ReturnsAsync(list);

        var clock = Mock.Of<IDateTime>(x => x.UtcNow == DateTime.UtcNow);
        var cfg = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["Internal:Key"] = "k" }).Build();
        var contentsClient = new Mock<IContentsClient>();
        var sut = new UserService(repo.Object, clock, cache.Object, cfg, contentsClient.Object);

        var one = await sut.GetAllAsync(CancellationToken.None);
        var two = await sut.GetAllAsync(CancellationToken.None);
        one.Should().NotBeNull();
        two.Should().BeSameAs(list);
        repo.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        cache.Verify(
            c => c.SetAsync("users:all", It.IsAny<List<UserDto>>(), It.IsAny<TimeSpan>(),
                It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Given_cache_enabled_When_getbyid_twice_Then_repo_called_once_and_cache_hit_on_second()
    {
        var id = Guid.NewGuid();
        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = id, Email = "e@x.com", FullName = "Eve", CreatedAtUtc = DateTime.UtcNow });

        var cache = new Mock<ICacheService>();
        var dto = new UserDto(id, "e@x.com", "Eve", DateTime.UtcNow);
        cache.SetupSequence(c => c.GetAsync<UserDto>($"users:id:{id}", It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserDto?)null)
            .ReturnsAsync(dto);

        var clock = Mock.Of<IDateTime>(x => x.UtcNow == DateTime.UtcNow);
        var cfg = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["Internal:Key"] = "k" }).Build();
        var contentsClient = new Mock<IContentsClient>();
        var sut = new UserService(repo.Object, clock, cache.Object, cfg, contentsClient.Object);

        _ = await sut.GetByIdAsync(id, CancellationToken.None);
        var two = await sut.GetByIdAsync(id, CancellationToken.None);
        two.Should().BeSameAs(dto);
        repo.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Given_nonexistent_user_When_update_Then_returns_null()
    {
        await using var db = InMemoryDb();
        var repo = new UserRepository(db);
        var clock = Mock.Of<IDateTime>(x => x.UtcNow == DateTime.UtcNow);
        var cache = new Mock<ICacheService>();
        var cfg = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["Internal:Key"] = "k" }).Build();
        var contentsClient = new Mock<IContentsClient>();
        var sut = new UserService(repo, clock, cache.Object, cfg, contentsClient.Object);

        var res = await sut.UpdateAsync(Guid.NewGuid(), new UpdateUserRequest("x@y.com", "X"), CancellationToken.None);
        res.Should().BeNull();
    }
}