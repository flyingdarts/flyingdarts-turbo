namespace Flyingdarts.Backend.Games.X01.Queue.Models;

public class QueueMessage<TMessage>
{
    [JsonPropertyName("messages")]
    public List<TMessage>? Messages { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime? CreatedAt { get; set; }

    [JsonPropertyName("is_proccessed")]
    public bool IsProcessed { get; set; }
    // ReSharper disable once PublicConstructorInAbstractClass
    public QueueMessage()
    {

    }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}
