using Flyingdarts.DynamoDb.Service;

namespace Flyingdarts.Connection.Services
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
            if (string.IsNullOrEmpty(connectionId))
            {
                throw new Exception(nameof(connectionId));
            }

            if (string.IsNullOrEmpty(playerId))
            {
                throw new Exception(nameof(playerId));
            }

            var user = await DynamoDbService.ReadUserAsync(playerId, cancellationToken);

            user.ConnectionId = connectionId;

            await DynamoDbService.WriteUserAsync(user, cancellationToken);
        }
    }
}
