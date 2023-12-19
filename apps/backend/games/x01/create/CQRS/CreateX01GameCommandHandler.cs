using Microsoft.Extensions.Options;

namespace Flyingdarts.Backend.Games.X01.Create.CQRS;

public class CreateX01GameCommandHandler : IRequestHandler<CreateX01GameCommand, APIGatewayProxyResponse>
{
    private readonly IDynamoDBContext _dbContext;
    private readonly ApplicationOptions _applicationOptions;
    private readonly CachingService<X01State> _cachingService;

    public CreateX01GameCommandHandler(IDynamoDBContext dbContext, IOptions<ApplicationOptions> applicationOptions, CachingService<X01State> cachingService)
    {
        _dbContext = dbContext;
        _applicationOptions = applicationOptions.Value;
        _cachingService = cachingService;
    }

    public async Task<APIGatewayProxyResponse> Handle(CreateX01GameCommand request, CancellationToken cancellationToken)
    {
        var socketMessage = new SocketMessage<CreateX01GameCommand>
        {
            Action = "games/x01/create",
            Message = request
        };

        // todo: update connection id via service
        var game = await CreateGame(request.Sets, request.Legs, cancellationToken);

        socketMessage.Message!.GameId = game.GameId.ToString();


        return new APIGatewayProxyResponse
        {
            StatusCode = 200,
            Body = JsonSerializer.Serialize(socketMessage)
        };
    }

    private async Task<Game> CreateGame(int sets, int legs, CancellationToken cancellationToken)
    {
        var game = Game.Create(2, X01GameSettings.Create(sets, legs));

        _cachingService.State = X01State.Create(game.GameId);
        _cachingService.AddGame(game);
        await _cachingService.Save(cancellationToken);

        var gameWrite = _dbContext.CreateBatchWrite<Game>(_applicationOptions.ToOperationConfig());
        gameWrite.AddPutItem(game);

        await gameWrite.ExecuteAsync(cancellationToken);
        return game;
    }
}