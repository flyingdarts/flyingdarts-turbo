namespace Flyingdarts.Persistence
{
    public class UserProfile : IUserProfile
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Country { get; set; }
        public string Picture { get; set; }

        public UserProfile()
        {
            // required for dynamodb
        }

        public UserProfile(string userName, string email, string country, string picture)
        {
            UserName = userName;
            Email = email;
            Country = country;
            Picture = picture;
        }

        public static UserProfile Create(
            string userName,
            string email,
            string country,
            string picture
        )
        {
            return new UserProfile
            {
                UserName = userName,
                Email = email,
                Country = country,
                Picture = picture
            };
        }
    }
}
