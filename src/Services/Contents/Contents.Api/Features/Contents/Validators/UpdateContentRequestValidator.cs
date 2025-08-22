namespace Contents.Api.Features.Contents.Validators;

public sealed class UpdateContentRequestValidator : AbstractValidator<UpdateContentRequest>
{
    public UpdateContentRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Body).NotEmpty().MaximumLength(4000);
    }
}