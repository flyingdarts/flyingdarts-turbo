// See https://aka.ms/new-console-template for more information

using System.Text.Json;
using Flyingdarts.Persistence;

Console.WriteLine("Hello, World!");

var seedService = new SeedDataService();
var users = seedService.GenerateUsers();
seedService.SimulateGames(users);

public class SeedDataService
{
    public List<User> GenerateUsers()
    {
        var users = new List<User>
        {
            User.Create("auth_1", "con_1", UserProfile.Create("player_1", "email_1", "be")),
            User.Create("auth_2", "con_2", UserProfile.Create("player_2", "email_2", "nl")),
        };

        return users;
    }

    public void SimulateGames(List<User> users)
    {
        var games = new List<Game>();
        var players = new List<GamePlayer>();
        var darts = new List<GameDart>();

        Random rnd = new Random();

        for (int day = 1; day < 8; day++)
        {
            for (int gameCount = 0; gameCount < 3; gameCount++)
            {
                var game = new Game
                {
                    GameId = new DateTime(2024, 2, 3, 21, 0 + gameCount, 53).Ticks,
                    X01 = new X01GameSettings
                    {
                        Legs = 5,
                        Sets = 3,
                        StartingScore = 501,
                        DoubleIn = false,
                        DoubleOut = true
                    }
                };

                // Assuming GamePlayer class links a User to a Game and can track wins/losses
                var player1 = new GamePlayer { PlayerId = users[0].UserId, GameId = game.GameId, CreatedAt = new DateTime(2024, 2, day) };
                var player2 = new GamePlayer { PlayerId = users[1].UserId, GameId = game.GameId, CreatedAt = new DateTime(2024, 2, day) };
                players.AddRange(new List<GamePlayer> { player1, player2 });

                // Here we can simulate some darts for each game and add them to the darts list
                int dartsPerPlayer = 21; // Assuming a standard number of darts per game per player
                for (int dartIndex = 0; dartIndex < dartsPerPlayer; dartIndex++)
                {
                    // Randomly generates scores typically found in a darts game
                    var score = rnd.Next(0, 61) * 3; // Dart scores range from 0 (miss) to 60 (triple 20) times 3

                    var dartPlayer1 = new GameDart { GameId = game.GameId, PlayerId = player1.PlayerId, Score = score };
                    var dartPlayer2 = new GameDart { GameId = game.GameId, PlayerId = player2.PlayerId, Score = score };
                    darts.Add(dartPlayer1);
                    darts.Add(dartPlayer2);
                }
            }
            
        }
        OutputGameData(games, players, darts);
    }
     private void OutputGameData(List<Game> games, List<GamePlayer> players, List<GameDart> darts)
    {
        Console.WriteLine("Games:");
        foreach (var game in games)
        {
            Console.WriteLine(JsonSerializer.Serialize(game));
        }

        Console.WriteLine("\nPlayers:");
        foreach (var player in players)
        {
            Console.WriteLine(JsonSerializer.Serialize(player));
        }

        Console.WriteLine("\nDarts:");
        foreach (var dart in darts)
        {
            Console.WriteLine(JsonSerializer.Serialize(dart));
        }
    }
}