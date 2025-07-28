namespace Flyingdarts.Persistence
{
    [DynamoDBTable("Flyingdarts-Application-Table")]
    public class User : IPrimaryKeyItem, ISortKeyItem, IAlternativeSortKeyItem
    {
        [DynamoDBHashKey("PK")]
        public string PrimaryKey { get; set; }

        [DynamoDBRangeKey("SK")]
        public string SortKey { get; set; }

        [DynamoDBLocalSecondaryIndexRangeKey("LSI1")]
        public string LSI1 { get; set; }
        public string UserId { get; set; }
        public string ConnectionId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string AuthProviderUserId { get; set; }
        public UserProfile Profile { get; set; }
        public Guid MeetingIdentifier { get; set; }

        public User()
        {
            CreatedAt = DateTime.UtcNow;
            UserId = CreatedAt.Ticks.ToString();
            PrimaryKey = Constants.User;
        }

        public static User Create(string authProviderUserId, string connectionId, UserProfile userProfile)
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
