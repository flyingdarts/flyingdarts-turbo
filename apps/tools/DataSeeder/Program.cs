// See https://aka.ms/new-console-template for more information

using Flyingdarts.Persistence;

Console.WriteLine("Hello, World!");

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
        Random rnd = new Random();

        for (int day = 0; day < 30; day++)
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
                var player1 = new GamePlayer { PlayerId = users[0].UserId, GameId = game.GameId };
                var player2 = new GamePlayer { PlayerId = users[1].UserId, GameId = game.GameId };
                    
                game.GamePlayers.Add(player1);
                game.GamePlayers.Add(player2);
                    
                games.Add(game);
            }
        }

        // Here you could save the games to a database or output them somehow
    }
}