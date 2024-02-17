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
                    (DateTime StartDate, DateTime EndDate) GetCurrentMonthDateRange()
                    {
                        var startDate = DateTime.Parse(request.QueryStringParameters["startDate"]);
                        var endDate = DateTime.Parse(request.QueryStringParameters["endDate"]);
                        return (startDate, endDate);
                    }
                    var (startDate, endDate) = GetCurrentMonthDateRange();
                    
                    return await innerHandler.Handle(new GetStatsQuery()
                    {
                        UserId = request.RequestContext.Authorizer.GetValueOrDefault("UserId").ToString(), 
                        StartDate = startDate, 
                        EndDate = endDate
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