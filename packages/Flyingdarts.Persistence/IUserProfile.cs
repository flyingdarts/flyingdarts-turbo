namespace Flyingdarts.Persistence
{
    public interface IUserProfile
    {
        string UserName { get; set; }
        string Email { get; set; }
        string Country { get; set; }
    }
}
