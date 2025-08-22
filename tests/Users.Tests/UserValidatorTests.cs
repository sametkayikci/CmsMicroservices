using FluentValidation.TestHelper;
using Users.Api.Features.Users.Validators;

namespace Users.Tests;

public class UserValidatorTests
{
    [Fact]
    public void Given_invalid_email_When_validate_Then_has_error()
    {
        var v = new CreateUserRequestValidator();
        v.TestValidate(new CreateUserRequest("bad", "A")).ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Given_valid_update_When_validate_Then_ok()
    {
        var v = new UpdateUserRequestValidator();
        v.TestValidate(new UpdateUserRequest("sametkayikci61@gmail.com", "Samet")).IsValid.Should().BeTrue();
    }
}