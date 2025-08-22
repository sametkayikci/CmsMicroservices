namespace Contents.Test;

public class InternalControllerTests
{
    [Fact]
    public async Task Given_wrong_internal_key_When_delete_by_user_Then_401()
    {
        var svc = new Mock<IContentService>();
        var cfg = new ConfigurationBuilder().AddInMemoryCollection(
            new Dictionary<string, string?> { ["Internal:Key"] = "secret" }).Build();

        var ctrl = new InternalController(svc.Object, cfg)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
        };
        ctrl.HttpContext.Request.Headers["X-Internal-Key"] = "wrong";

        var res = await ctrl.DeleteByUser(Guid.NewGuid(), CancellationToken.None);
        Assert.IsType<UnauthorizedResult>(res);
    }

    [Fact]
    public async Task Given_correct_key_When_delete_by_user_Then_ok_with_count()
    {
        var svc = new Mock<IContentService>();
        svc.Setup(s => s.DeleteByUserAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(3);

        var cfg = new ConfigurationBuilder().AddInMemoryCollection(
            new Dictionary<string, string?> { ["Internal:Key"] = "secret" }).Build();

        var ctrl = new InternalController(svc.Object, cfg)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
        };
        ctrl.HttpContext.Request.Headers["X-Internal-Key"] = "secret";

        var res = await ctrl.DeleteByUser(Guid.NewGuid(), default) as OkObjectResult;
        Assert.NotNull(res);

        
        var prop = res!.Value!.GetType().GetProperty("affected",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
        Assert.NotNull(prop);
        var affected = (int)prop!.GetValue(res.Value)!;

        Assert.Equal(3, affected);
    }
}