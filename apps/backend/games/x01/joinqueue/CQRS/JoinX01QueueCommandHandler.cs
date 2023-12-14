using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Lambda.APIGatewayEvents;
using Flyingdarts.Backend.Shared.Models;
using Flyingdarts.Persistence;
using Flyingdarts.Shared;
using MediatR;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

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

        var qualifyingGames = await GetQualifyingGamesAsync(cancellationToken);

        var game = qualifyingGames.Any()
            ? qualifyingGames.First()
            : await CreateGame(request.Sets, request.Legs, cancellationToken);

        socketMessage.Message!.GameId = game.GameId.ToString();

        return new APIGatewayProxyResponse
        {
            StatusCode = 200,
            Body = JsonSerializer.Serialize(socketMessage)
        };
    }

    private async Task<List<Game>> GetQualifyingGamesAsync(CancellationToken cancellationToken)
    {
        var qualifyingGames = await _dbContext.FromQueryAsync<Game>(X01GamesQueryConfig(), _applicationOptions.ToOperationConfig())
            .GetRemainingAsync(cancellationToken);

        var groupedGames = qualifyingGames.GroupBy(x => x.GameId);

        var qualifyingGamesToReturn = groupedGames
            .Where(group => group.Count() == 1) // Filter out groups with more than one item
            .Select(group => group.Single()) // Select the single item in each group
            .ToList();

        return qualifyingGamesToReturn;
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

    private static QueryOperationConfig X01GamesQueryConfig()
    {
        var queryFilter = new QueryFilter("PK", QueryOperator.Equal, Constants.Game);
        return new QueryOperationConfig { Filter = queryFilter };
    }
}