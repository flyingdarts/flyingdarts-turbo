namespace Flyingdarts.Backend.Stats.Api.Requests.GetStats;

public class GetStatsValidator : AbstractValidator<GetStatsQuery>
{
    public GetStatsValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}