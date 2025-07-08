namespace Flyingdarts.Backend.Signalling.OnDisconnect.Models;

public static class APIGatewayProxyRequestExtensions
{
    public static SocketMessage<T> To<T>(this APIGatewayProxyRequest request, ILambdaSerializer serializer) where T : class
    {
        if (string.IsNullOrWhiteSpace(request.Body))
            return null;

        using var ms = new MemoryStream(Encoding.UTF8.GetBytes(request.Body));
        var deserializedResponse = serializer.Deserialize<SocketMessage<T>>(ms);
        deserializedResponse.ConnectionId = request.RequestContext.ConnectionId;

        return deserializedResponse;
    }
}