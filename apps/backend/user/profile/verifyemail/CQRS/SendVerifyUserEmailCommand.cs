using Amazon.Lambda.Core;
using MediatR;

public class SendVerifyUserEmailCommand : IRequest
{
    public string Email { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    internal ILambdaContext Context { get; set; }
}