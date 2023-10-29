using System;
using System.Threading;
using System.Threading.Tasks;
using Flyingdarts.Persistence;
using MediatR;

public class JoinRoomCommandHandler : IRequestHandler<JoinRoomCommand>
{
    private readonly DynamoDbService _dynamoDbService;

    public JoinRoomCommandHandler(DynamoDbService dynamoDbService)
    {
        _dynamoDbService = dynamoDbService;
    }
    public async Task Handle(JoinRoomCommand request, CancellationToken cancellationToken)
    {
       
    }
}