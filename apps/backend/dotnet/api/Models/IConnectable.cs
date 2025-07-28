namespace Flyingdarts.Backend.Games.X01.Api.Models;

/// <summary>
/// Interface for objects that can be connected via WebSocket connections.
/// Provides a contract for managing connection identifiers.
/// </summary>
public interface IConnectable
{
    /// <summary>
    /// Gets or sets the unique identifier for the WebSocket connection.
    /// This ID is used to route messages to the correct client connection.
    /// </summary>
    string? ConnectionId { get; set; }

    /// <summary>
    /// Gets a value indicating whether this object has a valid connection ID.
    /// </summary>
    bool IsConnected { get; }
}
