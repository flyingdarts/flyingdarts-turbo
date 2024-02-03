using System;
using Flyingdarts.Backend.Stats.Api.Requests.GetStats;

var services = ServiceFactory.GetServiceProvider();
var innerHandler = new InnerHandler(services);
var serializer = new DefaultLambdaJsonSerializer(x => x.PropertyNameCaseInsensitive = true);

// The function handler that will be called for each Lambda event
var handler = async (APIGatewayProxyRequest request, ILambdaContext context) =>
{
    try
    {
        if (request.Resource == "/stats")
        {
            switch (request.HttpMethod)
            {
                case "GET":
                    static (DateTime StartDate, DateTime EndDate, int Year, int Month) GetCurrentMonthDateRange()
                    {
                        var today = DateTime.Today;
                        var startDate = new DateTime(today.Year, today.Month, 1);
                        var endDate = startDate.AddMonths(1).AddDays(-1);
                        return (startDate, endDate, today.Year, today.Month);
                    }
                    var (startDate, endDate, year, month) = GetCurrentMonthDateRange();
                    
                    return await innerHandler.Handle(new GetStatsQuery()
                    {
                        UserId = request.RequestContext.Authorizer.GetValueOrDefault("UserId").ToString(), 
                        StartDate = startDate, 
                        EndDate = endDate,
                        Year = year,
                        Month = month
                    });
               }
        }
    }
    catch (Exception ex)
    {
        return new APIGatewayProxyResponse
        {
            StatusCode = 400,
            Body = $"{ex.Message}\n {ex.StackTrace}"
        };
    }

    return new APIGatewayProxyResponse
    {
        StatusCode = 404,
        Body = "Resource not found"
    };
};

await LambdaBootstrapBuilder.Create(handler, serializer)
    .Build()
    .RunAsync();