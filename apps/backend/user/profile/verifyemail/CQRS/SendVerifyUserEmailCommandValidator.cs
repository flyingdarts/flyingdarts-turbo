using FluentValidation;

public class SendVerifyUserEmailCommandValidator : AbstractValidator<SendVerifyUserEmailCommand>
{
    public SendVerifyUserEmailCommandValidator()
    {
        RuleFor(x => x.Email).EmailAddress();
        RuleFor(x => x.Subject).NotEmpty();
        RuleFor(x => x.Body).NotEmpty();
    }
}