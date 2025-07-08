namespace Flyingdarts.Backend.User.Profile.Api.Requests.Create;

public class CreateUserProfileCommandValidator : AbstractValidator<CreateUserProfileCommand>
{
    public CreateUserProfileCommandValidator()
    {
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