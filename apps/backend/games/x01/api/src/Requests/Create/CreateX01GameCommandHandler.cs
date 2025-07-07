using Flyingdarts.Connection.Services;
using Flyingdarts.Meetings.Service.Services;
using Flyingdarts.Metadata.Services.Services.X01;

namespace Flyingdarts.Backend.Games.X01.Api.Requests.Create;

public record CreateX01GameCommandHandler(
    IDynamoDBContext DbContext,
    CachingService<X01State> CachingService,
    ConnectionService ConnectionService,
    IMeetingService MeetingService,
    X01MetadataService MetadataService
) : IRequestHandler<CreateX01GameCommand, APIGatewayProxyResponse>
{
    public async Task<APIGatewayProxyResponse> Handle(
        CreateX01GameCommand request,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(request);

        var socketMessage = new SocketMessage<CreateX01GameCommand>
        {
            Action = "games/x01/create",
            Message = request
        };

        // Update connection ID
        await ConnectionService.UpdateConnectionIdAsync(
            request.PlayerId,
            request.ConnectionId,
            cancellationToken
        );

        // Create game
        var game = await CreateGameAsync(request.Sets, request.Legs, cancellationToken);

        // Update message with game ID
        socketMessage.Message!.GameId = game.GameId.ToString();

        // Populate metadata as the final step
        socketMessage.Metadata = (
            await MetadataService.GetMetadataAsync(game.GameId.ToString(), cancellationToken)
        ).ToDictionary();

        return new APIGatewayProxyResponse
        {
            StatusCode = 200,
            Body = JsonSerializer.Serialize(socketMessage)
        };
    }

    private async Task<Game> CreateGameAsync(
        int sets,
        int legs,
        CancellationToken cancellationToken
    )
    {
        var game = Game.Create(2, X01GameSettings.Create(sets, legs));

        // Initialize cache state
        CachingService.State = X01State.Create(game.GameId);
        CachingService.AddGame(game);
        await CachingService.Save(cancellationToken);

        // Write to database
        var gameWrite = DbContext.CreateBatchWrite<Game>(GetOperationConfig());
        gameWrite.AddPutItem(game);
        await gameWrite.ExecuteAsync(cancellationToken);

        return game;
    }

    private static DynamoDBOperationConfig GetOperationConfig() =>
        new()
        {
            OverrideTableName =
                $"Flyingdarts-Application-Table-{Environment.GetEnvironmentVariable("EnvironmentName")}"
        };
}
