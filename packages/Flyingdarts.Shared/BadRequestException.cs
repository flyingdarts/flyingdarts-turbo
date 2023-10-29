using System.Net;

public class BadRequestException : Exception
{
    public static HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
    public BadRequestException(string information, Type owner) : base(
        $"Error {(int)StatusCode}: {owner} generated an invalid request. Reason: {information}")
    {

    }
}