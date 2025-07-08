using Amazon.SQS;
using Amazon.SQS.Model;
using MediatR.Pipeline;

namespace Flyingdarts.Backend.User.Profile.Api.Requests.Update
{
    public class UpdateUserProfileCommandSendVerificationEmailHandler : IRequestPostProcessor<UpdateUserProfileCommand, APIGatewayProxyResponse>
    {
        public async Task Process(UpdateUserProfileCommand request, APIGatewayProxyResponse response, CancellationToken cancellationToken)
        {
            if (request.Email != "mike.pattyn1@gmail.com" &&
                request.Email != "support@flyingdarts.net" &&
                request.Email != "mike@flyingdarts.net")
                return;

            var sqsClient = new AmazonSQSClient();
            var messageRequest = new SendMessageRequest
            {
                MessageBody = JsonSerializer.Serialize(new
                {
                    request.Email,
                    Subject = "Flyingdarts: Verify your email!",
                    Body = "Body from UpdateUserProfileVerifyEmail"
                }),
                QueueUrl = System.Environment.GetEnvironmentVariable("SqsQueueUrl")
            };

            await sqsClient.SendMessageAsync(messageRequest, cancellationToken);
        }
    }
}
