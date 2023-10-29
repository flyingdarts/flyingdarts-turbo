using System.Net;

namespace Flyingdarts.Backend.Shared.Models;

public static class Responses
{
    private static APIGatewayProxyResponse Generate(string body, HttpStatusCode code = HttpStatusCode.OK) => new()
    {
        StatusCode = (int)code,
        Body = body
    };

    public static APIGatewayProxyResponse Created(string body) => Generate(body);
    public static APIGatewayProxyResponse InternalServerError(string body) => Generate(body, HttpStatusCode.InternalServerError);
}