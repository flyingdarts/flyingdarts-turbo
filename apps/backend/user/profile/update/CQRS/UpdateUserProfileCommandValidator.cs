using FluentValidation;

public class UpdateUserProfileCommandValidator : AbstractValidator<UpdateUserProfileCommand>
{
    public UpdateUserProfileCommandValidator()
    {
        RuleFor(x => x.UserId)
                        .Must(x => long.TryParse(x, out var number));

        RuleFor(x => x.UserName)
            .MinimumLength(2)
            .MaximumLength(32);

        RuleFor(x => x.Email)
            .EmailAddress();

        RuleFor(x => x.Country)
            .MinimumLength(2)
            .MaximumLength(3);
    }
}