namespace Flyingdarts.Backend.Games.X01.Create.CQRS;

public record CreateX01GameCommandHandler(DynamoDbService dynamoDbService) : IRequestHandler<CreateX01GameCommand, APIGatewayProxyResponse>
{
    public async Task<APIGatewayProxyResponse> Handle(CreateX01GameCommand request, CancellationToken cancellationToken)
    {
        var socketMessage = new SocketMessage<CreateX01GameCommand>
        {
            Action = "games/x01/create",
            Message = request
        };
        
        // todo: update connection id via service
        
        var game = Game.Create(2, X01GameSettings.Create(request.Sets, request.Legs));
        
        await dynamoDbService.WriteGameAsync(game, cancellationToken);
        
        return new APIGatewayProxyResponse
        {
            StatusCode = 200,
            Body = JsonSerializer.Serialize(socketMessage)
        };
    }
}