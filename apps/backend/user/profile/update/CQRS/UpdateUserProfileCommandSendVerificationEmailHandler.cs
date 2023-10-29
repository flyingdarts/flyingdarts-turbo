using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Lambda;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Model;
using Amazon.SQS;
using Amazon.SQS.Model;
using MediatR.Pipeline;

namespace Flyingdarts.Backend.User.Profile.Update.CQRS
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
                    Email = request.Email,
                    Subject = "Flyingdarts: Verify your email!",
                    Body = "Body from UpdateUserProfileVerifyEmail"
                }),
                QueueUrl = System.Environment.GetEnvironmentVariable("SqsQueueUrl")
            };

            await sqsClient.SendMessageAsync(messageRequest, cancellationToken);
        }
    }
}
