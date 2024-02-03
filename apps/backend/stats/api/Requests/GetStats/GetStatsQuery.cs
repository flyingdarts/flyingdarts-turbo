using System;

namespace Flyingdarts.Backend.Stats.Api.Requests.GetStats;

public class GetStatsQuery : IRequest<APIGatewayProxyResponse>
{
    public string UserId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
}