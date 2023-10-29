namespace Flyingdarts.Backend.Shared.Models;
public class SocketMessage<TMessage>
{
    [JsonPropertyName("action")]
    public string? Action { get; set; } = null;

    [JsonPropertyName("message")]
    public TMessage? Message { get; set; }

    [JsonPropertyName("metadata")]
    public Dictionary<string, object>? Metadata { get; set; } = null;

    [JsonIgnore]
    public string? ConnectionId { get; set; } = null;

    // ReSharper disable once PublicConstructorInAbstractClass
    public SocketMessage()
    {

    }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}
