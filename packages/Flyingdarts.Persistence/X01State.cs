namespace Flyingdarts.Persistence
{
    [DynamoDBTable("Flyingdarts-State-Table")]
    public class X01State : IPrimaryKeyItem, ISortKeyItem, IAlternativeSortKeyItem
    {
        [DynamoDBHashKey("PK")]
        public string PrimaryKey { get; set; }

        [DynamoDBRangeKey("SK")]
        public string SortKey { get; set; }
        public string GameId { get; set; }
        public string CreatedAt { get; set; }
        public Game Game { get; set; }
        public List<GameDart> Darts { get; set; }
        public List<GamePlayer> Players { get; set; }
        public List<User> Users {get;set;}
        public X01State()
        {
            PrimaryKey = $"X01State";
            SortKey = $"{GameId}";
        }

        public User()
        {
            CreatedAt = DateTime.UtcNow;
            UserId = CreatedAt.Ticks.ToString();
            PrimaryKey = Constants.User;
        }

        public static User Create(string cognitoUserId, string cognitoUserName, string connectionId, UserProfile userProfile)
        {
            var user = new User()
            {
                CognitoUserId = cognitoUserId,
                CognitoUserName = cognitoUserName,
                ConnectionId = connectionId,
                Profile = userProfile
            };
            user.SortKey = $"{user.UserId}#{userProfile.Country}";
            user.LSI1 = $"{user.CognitoUserName}#{user.CreatedAt}";
            return user;
        }
    }
}
