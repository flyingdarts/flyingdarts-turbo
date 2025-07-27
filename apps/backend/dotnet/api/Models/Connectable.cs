namespace Flyingdarts.Backend.Games.X01.Api.Models;

/// <summary>
/// Abstract base class for objects that can be connected via WebSocket connections.
/// Provides a common interface for managing connection identifiers.
/// </summary>
public abstract class Connectable : IConnectable
{
    private string? _connectionId;

    /// <summary>
    /// Gets or sets the unique identifier for the WebSocket connection.
    /// This ID is used to route messages to the correct client connection.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when attempting to set an empty or whitespace connection ID.</exception>
    public string? ConnectionId
    {
        get => _connectionId;
        set
        {
            if (value is not null && string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Connection ID cannot be empty or whitespace.", nameof(value));
            }
            _connectionId = value;
        }
    }

    /// <summary>
    /// Gets a value indicating whether this object has a valid connection ID.
    /// </summary>
    public bool IsConnected => !string.IsNullOrWhiteSpace(_connectionId);

    /// <summary>
    /// Initializes a new instance of the Connectable class.
    /// </summary>
    protected Connectable()
    {
    }

    /// <summary>
    /// Initializes a new instance of the Connectable class with a connection ID.
    /// </summary>
    /// <param name="connectionId">The connection identifier.</param>
    protected Connectable(string? connectionId)
    {
        ConnectionId = connectionId;
    }
}