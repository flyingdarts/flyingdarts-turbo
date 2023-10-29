using FluentValidation;

public class GetUserProfileQueryValidator : AbstractValidator<GetUserProfileQuery>
{
    public GetUserProfileQueryValidator()
    {
        RuleFor(x => x.CognitoUserName).NotNull().NotEmpty();
    }
}