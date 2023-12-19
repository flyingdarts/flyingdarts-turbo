using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.SQS;
using Amazon.SQS.Model;
using Flyingdarts.Backend.Games.X01.JoinQueue;
using Flyingdarts.Backend.Shared.Models;
using Flyingdarts.Persistence;
using Flyingdarts.Shared;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using static Flyingdarts.Shared.Lambdas.Functions.User;

public class

    JoinX01QueueCommandHandler : IRequestHandler<JoinX01QueueCommand, APIGatewayProxyResponse>
{
    private readonly IDynamoDBContext _dbContext;
    private readonly ApplicationOptions _applicationOptions;
    private readonly CachingService<X01State> _cachingService;

    public JoinX01QueueCommandHandler(IDynamoDBContext dbContext, IOptions<ApplicationOptions> applicationOptions, CachingService<X01State> cachingService)
    {
        _dbContext = dbContext;
        _applicationOptions = applicationOptions.Value;
        _cachingService = cachingService;
    }
    public async Task<APIGatewayProxyResponse> Handle(JoinX01QueueCommand request, CancellationToken cancellationToken)
    {
        var socketMessage = new SocketMessage<JoinX01QueueCommand>
        {
            Action = "games/x01/joinqueue",
            Message = request
        };

        var queueState = X01Queue.Create(request.PlayerId, -1, request.ConnectionId, X01GameSettings.Create(request.Sets, request.Legs));

        var queueWrite = _dbContext.CreateBatchWrite<X01Queue>(OperationConfig);

        queueWrite.AddPutItem(queueState);

        await queueWrite.ExecuteAsync(cancellationToken);

        return new APIGatewayProxyResponse
        {
            StatusCode = 200,
            Body = JsonSerializer.Serialize(socketMessage)
        };
    }

    private DynamoDBOperationConfig OperationConfig
    {
        get
        {
            var tableName = $"Flyingdarts-X01Queue-Table-{Environment.GetEnvironmentVariable("EnvironmentName")}";
            return new DynamoDBOperationConfig { OverrideTableName = tableName };
        }
    }
}