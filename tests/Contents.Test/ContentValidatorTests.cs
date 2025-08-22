namespace Contents.Test;

public class ContentValidatorTests
{
    [Fact]
    public void Given_empty_title_When_validate_Then_has_error()
    {
        var v = new CreateContentRequestValidator();
        v.TestValidate(new CreateContentRequest(Guid.NewGuid(), "", "b")).ShouldHaveAnyValidationError();
    }

    [Fact]
    public void Given_valid_update_When_validate_Then_ok()
    {
        var v = new UpdateContentRequestValidator();
        v.TestValidate(new UpdateContentRequest("Title", "Body")).IsValid.Should().BeTrue();
    }
}