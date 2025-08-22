namespace Contents.Test;

public class ContentsControllerTests
{
    [Fact]
    public async Task Given_missing_content_When_getbyid_Then_404()
    {
        var svc = new Mock<IContentService>();
        svc.Setup(s => s.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ContentDto?)null);

        var ctrl = new ContentsController(svc.Object);
        var res = await ctrl.GetById(Guid.NewGuid(), CancellationToken.None);
        Assert.IsType<NotFoundResult>(res);
    }

    [Fact]
    public async Task Given_valid_request_When_create_Then_201()
    {
        var svc = new Mock<IContentService>();
        var dto = new ContentDto(Guid.NewGuid(), Guid.NewGuid(), "T", "B", DateTime.UtcNow);
        svc.Setup(s => s.CreateAsync(It.IsAny<CreateContentRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dto);

        var ctrl = new ContentsController(svc.Object);
        var res = await ctrl.Create(new CreateContentRequest(Guid.NewGuid(), "T", "B"), CancellationToken.None);
        var created = Assert.IsType<CreatedAtActionResult>(res);
        created.Value.Should().Be(dto);
    }
}