using System;
using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;
using System.Threading;
using System.Threading.Tasks;
using Flyingdarts.Persistence;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using Amazon.ApiGatewayManagementApi.Model;
using System.IO;
using System.Text;
using Amazon.ApiGatewayManagementApi;
using Flyingdarts.Backend.Shared.Dtos;
using Flyingdarts.Backend.Shared.Models;

public record JoinX01GameCommandHandler(IDynamoDbService DynamoDbService, IAmazonApiGatewayManagementApi ApiGatewayClient) : IRequestHandler<JoinX01GameCommand, APIGatewayProxyResponse>
{
    public async Task<APIGatewayProxyResponse> Handle(JoinX01GameCommand request, CancellationToken cancellationToken)
    {
        var socketMessage = new SocketMessage<JoinX01GameCommand>
        {
            Action = "v2/games/x01/join",
            Message = request
        };

        await UpdateConnectionId(socketMessage, cancellationToken);

        request.Game = await DynamoDbService.ReadGameAsync(long.Parse(request.GameId), cancellationToken);

        if (request.Game is not null)
        {
            var player = GamePlayer.Create(long.Parse(request.GameId), request.PlayerId);

            await DynamoDbService.WriteGamePlayerAsync(player, cancellationToken);
        }

        request.Players = await DynamoDbService.ReadGamePlayersAsync(long.Parse(request.GameId), cancellationToken);

        if (request.Players.Count == 2)
        {
            await UpdateGameStatus(request.Game, GameStatus.Started, cancellationToken);
        }
        
        request.Users = await DynamoDbService.ReadUsersAsync(request.Players.Select(x => x.PlayerId).ToArray(), cancellationToken);
        request.Darts = await DynamoDbService.ReadGameDartsAsync(long.Parse(request.GameId), cancellationToken);
        
        socketMessage.Metadata = CreateMetaData(request.Game, request.Darts, request.Players, request.Users);
        
        if (HasAnyPlayerWon(request.Darts, request.Game!.X01.Legs, request.Game.X01.Sets,request.Players.Select(x => x.PlayerId.ToString()).ToList()))
        {
            socketMessage.Metadata!["NextPlayer"] = null;
            socketMessage.Metadata!["WinningPlayer"] = GetWinningPlayer(request.Darts, request.Game.X01.Legs,
                request.Game.X01.Sets, request.Players.Select(x => x.PlayerId.ToString()).ToList());

            request.Game.Status = GameStatus.Finished;

            await DynamoDbService.WriteGameAsync(request.Game, cancellationToken);
        }
        
        await NotifyRoomAsync(socketMessage, cancellationToken);

        return new APIGatewayProxyResponse { StatusCode = 200, Body = JsonSerializer.Serialize(socketMessage) };
    }
    public static string GetWinningPlayer(List<GameDart> darts, int legsRequired, int bestOfSets, List<string> playerIds)
    {
        foreach (var playerId in playerIds)
        {
            int setsWonByPlayer = CalculateSets(darts, playerId, legsRequired);
            int setsRequiredToWin = (bestOfSets + 1) / 2;

            // If the player has won the required number of sets in a "best of" scenario, they've won the game.
            if (setsWonByPlayer >= setsRequiredToWin)
            {
                return playerId;
            }
        }

        // If no player has won enough sets, return null to indicate that the game is still ongoing.
        return null;
    }
    public async Task UpdateConnectionId(SocketMessage<JoinX01GameCommand> message, CancellationToken cancellationToken)
    {
        var user = await DynamoDbService.ReadUserAsync(message.Message!.PlayerId, cancellationToken);

        user.ConnectionId = message.Message.ConnectionId;

        await DynamoDbService.WriteUserAsync(user, cancellationToken);
    }
    public async Task NotifyRoomAsync(SocketMessage<JoinX01GameCommand> message, CancellationToken cancellationToken)
    {
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message)));

        foreach (var user in message.Message.Users)
        {
            if (!string.IsNullOrEmpty(user.ConnectionId))
            {
                var connectionId = user.UserId == message.Message.PlayerId
                    ? message.Message.ConnectionId : user.ConnectionId;

                var postConnectionRequest = new PostToConnectionRequest
                {
                    ConnectionId = connectionId,
                    Data = stream
                };

                stream.Position = 0;

                await ApiGatewayClient.PostToConnectionAsync(postConnectionRequest, cancellationToken);
            }
        }
    }
    public async Task UpdateGameStatus(Game game, GameStatus gameStatus, CancellationToken cancellationToken)
    {
        game.Status = gameStatus;
        game.LSI1 = $"{gameStatus}#{game.GameId}";
        game.SortKey = $"{game.GameId}#{gameStatus}";
        
        await DynamoDbService.WriteGameAsync(game, cancellationToken);
    }
    public static Dictionary<string, object> CreateMetaData(Game game, List<GameDart> darts, List<GamePlayer> players, List<User> users)
    {
        Metadata data = new Metadata();

        if (game is not null)
        {
            data.Game = new GameDto
            {
                Id = game.GameId.ToString(),
                PlayerCount = game.PlayerCount,
                Status = (GameStatusDto)(int)game.Status,
                Type = (GameTypeDto)(int)game.Type,
                X01 = new X01GameSettingsDto
                {
                    DoubleIn = game.X01.DoubleIn,
                    DoubleOut = game.X01.DoubleOut,
                    Legs = game.X01.Legs,
                    Sets = game.X01.Sets,
                    StartingScore = game.X01.StartingScore
                }
            };
        }

        if (darts is not null)
        {
            data.Darts = new();
            players.ForEach(p =>
            {
                data.Darts.Add(p.PlayerId, new());
                data.Darts[p.PlayerId] = darts
                    .OrderBy(x => x.CreatedAt)
                    .Where(x => x.PlayerId == p.PlayerId)
                    .Select(x => new DartDto
                    {
                        Id = x.Id,
                        Score = x.Score,
                        GameScore = x.GameScore,
                        Set = x.Set,
                        Leg= x.Leg,
                        CreatedAt = x.CreatedAt.Ticks
                    })
                    .ToList();
            });
        }

        if (players is not null)
        {
            var orderedPlayers = players.Select(x =>
            {
                return new PlayerDto
                {
                    PlayerId = x.PlayerId,
                    PlayerName = users.Single(y => y.UserId == x.PlayerId).Profile.UserName,
                    Country = users.Single(y => y.UserId == x.PlayerId).Profile.Country.ToLower(),
                    CreatedAt = x.PlayerId,
                    Legs = CalculateLegs(darts!, x.PlayerId).ToString(),
                    Sets = CalculateSets(darts!, x.PlayerId, data.Game.X01.Legs).ToString()
                };
            }).OrderBy(x => x.CreatedAt);
            
            data.Players = orderedPlayers;
        }       
        
        DetermineNextPlayer(data);

        try
        {
            var lastFinisher = darts!.OrderBy(x => x.CreatedAt).Last(x => x.GameScore == 0);
            data.Darts[data.Darts.Keys.First()] =
                data.Darts[data.Darts.Keys.First()].Where(x => x.CreatedAt > lastFinisher.CreatedAt.Ticks).ToList();

            data.Darts[data.Darts.Keys.Last()] =
                data.Darts[data.Darts.Keys.Last()].Where(x => x.CreatedAt > lastFinisher.CreatedAt.Ticks).ToList();
        } catch { }
        
        return data.toDictionary();
    }
    public static int CalculateLegs(List<GameDart> darts, string playerId)
    {
        // Group darts by set and then by player.
        var sets = darts.GroupBy(dart => dart.Set);

        var totalLegsWon = 0;

        // Calculate legs won in each set
        foreach (var set in sets)
        {
            var legsInSet = set.GroupBy(dart => dart.Leg);
            foreach (var leg in legsInSet)
            {
                // Check if the player has won the leg by being the first to finish (score of 0)
                if (leg.Any(dart => dart.PlayerId == playerId && dart.GameScore == 0))
                {
                    totalLegsWon++;
                }
            }
        }

        return totalLegsWon;
    }

    public static int CalculateSets(List<GameDart> darts, string playerId, int legsPerSet)
    {
        // Group darts by set
        var sets = darts.GroupBy(dart => dart.Set);

        int totalSetsWon = 0;

        // Calculate sets won
        foreach (var set in sets)
        {
            int legsWonInCurrentSet = CalculateLegs(set.ToList(), playerId);
            if (legsWonInCurrentSet >= ((legsPerSet / 2) + 1)) // If the player has won the majority of the legs in the set
            {
                totalSetsWon++;
            }
        }

        return totalSetsWon;
    }

    public static bool HasPlayerWon(List<GameDart> darts, string playerId, int legsRequired, int bestOfSets)
    {
        int setsWon = CalculateSets(darts, playerId, legsRequired);
    
        int setsRequiredToWin = (bestOfSets + 1) / 2;
    
        // If the player has won the required number of sets in a "best of" scenario, they've won the game.
        return setsWon >= setsRequiredToWin;
    }
    public static bool HasAnyPlayerWon(List<GameDart> darts, int legsRequired, int bestOfSets, List<string> playerIds)
    {
        foreach (var playerId in playerIds)
        {
            int setsWonByPlayer = CalculateSets(darts, playerId, legsRequired);
            int setsRequiredToWin = (bestOfSets + 1) / 2;

            // If any player has won the required number of sets in a "best of" scenario, the game is won.
            if (setsWonByPlayer >= setsRequiredToWin)
            {
                return true;
            }
        }

        // If no player has won enough sets, the game is still ongoing.
        return false;
    }
    public static void DetermineNextPlayer(Metadata metadata)
    {
        if (metadata.Players.Count() == 2)
        {
            var p1_count = metadata.Darts[metadata.Players.First().PlayerId].Count();
            var p2_count = metadata.Darts[metadata.Players.Last().PlayerId].Count();
            if (p1_count > p2_count)
            {
                metadata.NextPlayer = metadata.Players.Last().PlayerId;
            }
            else
            {
                metadata.NextPlayer = metadata.Players.First().PlayerId;
            }
        }
    }
}
