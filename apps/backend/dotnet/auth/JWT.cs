namespace Flyingdarts.Backend.Auth;

public class JWT
{
    public string iss { get; set; } = string.Empty;
    public string sub { get; set; } = string.Empty;
    public int iat { get; set; }
    public int exp { get; set; }
    public string scope { get; set; } = string.Empty;
    public string azp { get; set; } = string.Empty;
    public string client_id { get; set; } = string.Empty;
}
