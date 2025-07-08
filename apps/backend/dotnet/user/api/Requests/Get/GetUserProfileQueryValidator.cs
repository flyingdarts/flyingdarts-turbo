namespace Flyingdarts.Backend.User.Profile.Api.Requests.Get;

public class GetUserProfileQueryValidator : AbstractValidator<GetUserProfileQuery>
{
    public GetUserProfileQueryValidator()
    {
        RuleFor(x => x.AuthProviderUserId).NotNull().NotEmpty();
    }
}