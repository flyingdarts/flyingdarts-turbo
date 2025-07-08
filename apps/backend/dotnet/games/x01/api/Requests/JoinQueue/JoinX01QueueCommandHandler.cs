using Flyingdarts.Connection.Services;
using Flyingdarts.Metadata.Services.Services.X01;

namespace Flyingdarts.Backend.Games.X01.Api.Requests.JoinQueue;

public record JoinX01QueueCommandHandler(
    IDynamoDBContext DbContext,
    ConnectionService ConnectionService,
    X01MetadataService MetadataService
) : IRequestHandler<JoinX01QueueCommand, APIGatewayProxyResponse>
{
    public async Task<APIGatewayProxyResponse> Handle(
        JoinX01QueueCommand request,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(request);

        var socketMessage = new SocketMessage<JoinX01QueueCommand>
        {
            Action = "games/x01/joinqueue",
            Message = request
        };

        // Update connection ID
        await ConnectionService.UpdateConnectionIdAsync(
            request.PlayerId,
            request.ConnectionId,
            cancellationToken
        );

        // Create and save queue state
        await CreateQueueStateAsync(request, cancellationToken);

        // Populate metadata as the final step (empty for queue operations)
        socketMessage.Metadata = new Dictionary<string, object>();

        return new APIGatewayProxyResponse
        {
            StatusCode = 200,
            Body = JsonSerializer.Serialize(socketMessage)
        };
    }

    private async Task CreateQueueStateAsync(
        JoinX01QueueCommand request,
        CancellationToken cancellationToken
    )
    {
        var queueState = X01Queue.Create(
            request.PlayerId,
            -1,
            request.ConnectionId,
            X01GameSettings.Create(request.Sets, request.Legs)
        );

        var queueWrite = DbContext.CreateBatchWrite<X01Queue>(GetOperationConfig());
        queueWrite.AddPutItem(queueState);
        await queueWrite.ExecuteAsync(cancellationToken);
    }

    private static DynamoDBOperationConfig GetOperationConfig() =>
        new()
        {
            OverrideTableName =
                $"Flyingdarts-X01Queue-Table-{Environment.GetEnvironmentVariable("EnvironmentName")}"
        };
}
