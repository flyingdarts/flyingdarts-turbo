﻿namespace Flyingdarts.Backend.Shared.Services
{
    public interface IConnectionService
    {
        Task UpdateConnectionIdAsync(string playerId, string connectionId, CancellationToken cancellationToken);
    }
}