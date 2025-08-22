namespace Users.Tests;

public class UsersControllerTests
{
    [Fact]
    public async Task Given_existing_user_When_getbyid_Then_returns_200()
    {
        var svc = new Mock<IUserService>();
        var dto = new UserDto(Guid.NewGuid(), "u@x.com", "User", DateTime.UtcNow);
        svc.Setup(s => s.GetByIdAsync(dto.Id, It.IsAny<CancellationToken>())).ReturnsAsync(dto);

        var ctrl = new UsersController(svc.Object);
        var res = await ctrl.GetById(dto.Id, CancellationToken.None);
        var ok = Assert.IsType<OkObjectResult>(res);
        ok.Value.Should().Be(dto);
    }

    [Fact]
    public async Task Given_missing_user_When_getbyid_Then_returns_404()
    {
        var svc = new Mock<IUserService>();
        svc.Setup(s => s.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((UserDto?)null);

        var ctrl = new UsersController(svc.Object);
        var res = await ctrl.GetById(Guid.NewGuid(), CancellationToken.None);
        Assert.IsType<NotFoundResult>(res);
    }

    [Fact]
    public async Task Given_valid_request_When_create_Then_returns_201_with_location()
    {
        var svc = new Mock<IUserService>();
        var dto = new UserDto(Guid.NewGuid(), "u@x.com", "User", DateTime.UtcNow);
        svc.Setup(s => s.CreateAsync(It.IsAny<CreateUserRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(dto);

        var ctrl = new UsersController(svc.Object);
        var res = await ctrl.Create(new CreateUserRequest("u@x.com", "User"), CancellationToken.None);
        var created = Assert.IsType<CreatedAtActionResult>(res);
        created.Value.Should().Be(dto);
        created.ActionName.Should().Be(nameof(UsersController.GetById));
    }
}