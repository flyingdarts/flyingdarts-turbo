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

public class QueueMessage<TMessage>
{
    [JsonPropertyName("messages")]
    public List<TMessage>? Messages { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime? CreatedAt { get;set;}

    [JsonPropertyName("is_proccessed")]
    public bool IsProcessed { get;set;}
    // ReSharper disable once PublicConstructorInAbstractClass
    public QueueMessage()
    {

    }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}