using System;
using System.Linq;
using Amazon.DynamoDBv2.DocumentModel;
using Flyingdarts.Backend.Stats.Api.Response;

namespace Flyingdarts.Backend.Stats.Api.Requests.GetStats;

public class GetStatsHandler : IRequestHandler<GetStatsQuery, APIGatewayProxyResponse>
{
    private readonly IDynamoDBContext _dbContext;
    private readonly IOptions<ApplicationOptions> _applicationOptions;

    public GetStatsHandler(IDynamoDBContext DbContext, IOptions<ApplicationOptions> ApplicationOptions)
    {
        _dbContext = DbContext;
        _applicationOptions = ApplicationOptions;
    }

    public async Task<APIGatewayProxyResponse> Handle(GetStatsQuery request, CancellationToken cancellationToken)
    {
        var user = (await _dbContext.FromQueryAsync<User>(QueryUserConfig(request.UserId),
            _applicationOptions.Value.ToOperationConfig()).GetRemainingAsync(cancellationToken)).Single();

        var queryItems = await _dbContext
            .FromQueryAsync<GameDart>(QueryConfig(user.UserId), _applicationOptions.Value.ToOperationConfig())
            .GetRemainingAsync(cancellationToken);

        if (!queryItems.Any())
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = 404
            };
        }

        queryItems = queryItems.Where(x => x.CreatedAt >= request.StartDate && x.CreatedAt < request.EndDate)
            .ToList();

        var grouped = queryItems.GroupBy(x => x.CreatedAt.Day)
            .Select(x => new StatsDart { Day = x.Key, Average = (int)x.Average(dart => dart.Score) })
            .ToList();

        FillMissingDays(grouped, request.Year, request.Month);

        var simplified = grouped.OrderBy(x=>x.Day).Select(x => new[] { x.Day, x.Average });
        
        return new APIGatewayProxyResponse
        {
            StatusCode = 200,
            Body = JsonSerializer.Serialize(simplified),
            Headers = new Dictionary<string, string>()
            {
                { "Access-Control-Allow-Origin", "*" },
                { "Access-Control-Allow-Methods", "*" },
                { "Access-Control-Allow-Headers", "Content-Type" }
            }
        };
    }

    private static void FillMissingDays(List<StatsDart> stats, int year, int month)
    {
        int daysInMonth = DateTime.DaysInMonth(year, month);
        var existingDays = new HashSet<int>(stats.Select(s => s.Day));

        for (int day = 1; day <= daysInMonth; day++)
        {
            if (!existingDays.Contains(day))
            {
                stats.Add(new StatsDart { Day = day, Average = 0 });
            }
        }
    }

    private static QueryOperationConfig QueryConfig(string userId)
    {
        var queryFilter = new QueryFilter("PK", QueryOperator.Equal, Constants.GameDart);
        queryFilter.AddCondition("LSI1", QueryOperator.BeginsWith, userId);
        return new QueryOperationConfig { Filter = queryFilter };
    }

    private static QueryOperationConfig QueryUserConfig(string userId)
    {
        var queryFilter = new QueryFilter("PK", QueryOperator.Equal, Constants.User);
        queryFilter.AddCondition("LSI1", QueryOperator.BeginsWith, userId);
        return new QueryOperationConfig { Filter = queryFilter };
    }
}