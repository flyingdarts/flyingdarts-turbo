namespace Flyingdarts.Persistence;

public interface IGameQueue<TState>
{
    public string PlayerId { get; set; }

    public X01GameSettings? X01 { get; set; }

    public int Average { get; set; }

    public DateTime Joined { get; set; }

    public string ConnectionId { get; set; }
}
