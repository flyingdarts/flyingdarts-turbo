namespace Flyingdarts.Backend.Auth;

public class JWT
{
    public string iss { get; set; }
    public string sub { get; set; }
    public int iat { get; set; }
    public int exp { get; set; }
    public string scope { get; set; }
    public string azp { get; set; }
    public string client_id { get; set; }
}
