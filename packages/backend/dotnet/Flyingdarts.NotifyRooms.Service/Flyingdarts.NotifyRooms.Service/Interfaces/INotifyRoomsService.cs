using Flyingdarts.Core.Models;

namespace Flyingdarts.NotifyRooms.Service.Interfaces;

public interface INotifyRoomsService
{
    Task NotifyRoomAsync<TNotification>(
        SocketMessage<TNotification> request,
        string[] connectionIds,
        CancellationToken cancellationToken
    );
}