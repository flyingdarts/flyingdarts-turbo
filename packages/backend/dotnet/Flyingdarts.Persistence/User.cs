namespace Flyingdarts.Persistence
{
    [DynamoDBTable("Flyingdarts-Application-Table")]
    public class User : IPrimaryKeyItem, ISortKeyItem, IAlternativeSortKeyItem
    {
        [DynamoDBHashKey("PK")]
        public string PrimaryKey { get; set; } = Constants.User;

        [DynamoDBRangeKey("SK")]
        public string SortKey { get; set; } = string.Empty;

        [DynamoDBLocalSecondaryIndexRangeKey("LSI1")]
        public string LSI1 { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string ConnectionId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string AuthProviderUserId { get; set; } = string.Empty;
        public UserProfile Profile { get; set; } = new UserProfile();
        public Guid MeetingIdentifier { get; set; }

        public User()
        {
            CreatedAt = DateTime.UtcNow;
            UserId = CreatedAt.Ticks.ToString();
        }

        public static User Create(
            string authProviderUserId,
            string connectionId,
            UserProfile userProfile
        )
        {
            var user = new User()
            {
                AuthProviderUserId = authProviderUserId,
                ConnectionId = connectionId,
                Profile = userProfile,
            };
            user.SortKey = $"{user.UserId}#{userProfile.Country}";
            user.LSI1 = $"{user.AuthProviderUserId}#{user.CreatedAt}";
            return user;
        }
    }
}
