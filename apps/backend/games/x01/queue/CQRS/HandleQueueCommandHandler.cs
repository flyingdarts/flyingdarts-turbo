using Flyingdarts.Backend.Games.X01.Queue.Messages;
using MediatR;

namespace Flyingdarts.Backend.Games.X01.Queue.CQRS;

public class HandleQueueCommandHandler : IRequestHandler<HandleQueueCommand>
{
    public Task Handle(HandleQueueCommand request, CancellationToken cancellationToken)
    {
        request.MatchingPlayers = new List<PlayerJoinedQueueMessage>();

        // Iterate through each player in the queue
        for (var i = 0; i < request.Messages.Count - 1; i++)
        {
            var currentPlayer = request.Messages[i];

            // Check if the player's average is within 10 of any other player in the remaining queue
            for (var j = i + 1; j < request.Messages.Count; j++)
            {
                var otherPlayer = request.Messages[j];

                // Check if the averages are within the specified range
                if (Math.Abs(currentPlayer.Average - otherPlayer.Average) <= 10)
                {
                    // Matching players found, add them to the result
                    request.MatchingPlayers.Add(currentPlayer);
                    request.MatchingPlayers.Add(otherPlayer);

                    // Remove matching players from the queue to avoid further matching
                    request.Messages.RemoveAt(j);
                    request.Messages.RemoveAt(i);

                    // Break from the inner loop as we've found a match for the current player
                    break;
                }
            }
        }
        
        // todo: write function that will notify the `request.MatchingPlayers` via the WebSocketApi
        // todo: write function that will remove the `request.MatchingPlayers` from the SQS queue.
        

        return Task.CompletedTask;
    }
}