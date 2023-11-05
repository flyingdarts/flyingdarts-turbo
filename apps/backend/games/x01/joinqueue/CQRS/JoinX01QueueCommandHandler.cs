using System;
using System.Linq;
using Amazon.Lambda.APIGatewayEvents;
using System.Threading;
using System.Threading.Tasks;
using Flyingdarts.Persistence;
using MediatR;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.DataModel;
using Flyingdarts.Shared;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Flyingdarts.Backend.Shared.Models;

public class JoinX01QueueCommandHandler : IRequestHandler<JoinX01QueueCommand, APIGatewayProxyResponse>
{
    private readonly IDynamoDBContext _dbContext;
    private readonly ApplicationOptions _applicationOptions;
    public JoinX01QueueCommandHandler(IDynamoDBContext dbContext, IOptions<ApplicationOptions> applicationOptions)
    {
        _dbContext = dbContext;
        _applicationOptions = applicationOptions.Value;
    }
    public async Task<APIGatewayProxyResponse> Handle(JoinX01QueueCommand request, CancellationToken cancellationToken)
    {
        var socketMessage = new SocketMessage<JoinX01QueueCommand>();
        socketMessage.Message = request;
        socketMessage.Action = "v2/games/x01/joinqueue";

        var qualifyingGames = await GetQualifyingGamesAsync(cancellationToken);

        var game = qualifyingGames.Any()
            ? qualifyingGames.First()
            : await CreateGame(cancellationToken);

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


    private async Task<Game> CreateGame(CancellationToken cancellationToken)
    {
        var game = Game.Create(2, X01GameSettings.Create(1, 3));
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

    private static string ConvertTicksToBase64()
    {
        long ticks = DateTime.UtcNow.Ticks;
        byte[] tickBytes = BitConverter.GetBytes(ticks);
        string base64String = Convert.ToBase64String(tickBytes);

        // Remove leading slashes if any
        while (base64String.StartsWith("/"))
        {
            base64String = base64String.Substring(1);
        }

        // Replace any special characters
        base64String = Regex.Replace(base64String, @"[^A-Za-z0-9]", "");

        // Add trailing '=' character if needed
        if (base64String.Length == 0 || base64String[base64String.Length - 1] != '=')
        {
            base64String += "=";
        }

        return base64String;
    }
}