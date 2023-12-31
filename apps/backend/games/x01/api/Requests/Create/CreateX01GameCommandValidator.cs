public class CreateX01GameCommandValidator : AbstractValidator<CreateX01GameCommand>
{
    public CreateX01GameCommandValidator()
    {
        RuleFor(x => x.Sets).Must(x => x % 2 == 1);
        RuleFor(x => x.Legs).Must(x => x % 2 == 1);
    }
}