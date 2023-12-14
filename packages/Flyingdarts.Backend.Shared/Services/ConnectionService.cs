using Flyingdarts.Persistence;

namespace Flyingdarts.Backend.Shared.Services
{
    public class ConnectionService : IConnectionService
    {
        public IDynamoDbService DynamoDbService { get; }

        public ConnectionService(IDynamoDbService dynamoDbService)
        {
            DynamoDbService = dynamoDbService;
        }
    
        public async Task UpdateConnectionIdAsync(string playerId, string connectionId, CancellationToken cancellationToken)
        {
            var user = await DynamoDbService.ReadUserAsync(playerId, cancellationToken);

            user.ConnectionId = connectionId;

            await DynamoDbService.WriteUserAsync(user, cancellationToken);
        }
    }
}
