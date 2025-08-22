namespace Contents.Api.Features.Contents.Validators;

public sealed class CreateContentRequestValidator : AbstractValidator<CreateContentRequest>
{
    public CreateContentRequestValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Body).NotEmpty().MaximumLength(4000);
    }
}