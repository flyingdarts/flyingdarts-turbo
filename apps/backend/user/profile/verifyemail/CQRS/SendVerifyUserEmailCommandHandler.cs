using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using MediatR;

public class SendVerifyUserEmailCommandHandler : IRequestHandler<SendVerifyUserEmailCommand>
{
    private readonly IAmazonSimpleEmailService _emailService;
    public SendVerifyUserEmailCommandHandler(IAmazonSimpleEmailService emailService)
    {
        _emailService = emailService;
    }
    public async Task Handle(SendVerifyUserEmailCommand request, CancellationToken cancellationToken)
    {
        // Build the email request
        var sendRequest = new SendEmailRequest
        {
            Source = "support@flyingdarts.net", // Replace with your email address
            Destination = new Destination
            {
                ToAddresses = new List<string> { request.Email } // Replace with the recipient's email address
            },
            Message = new Message
            {
                Subject = new Content(request.Subject), // Replace with your desired subject
                Body = new Body
                {
                    Text = new Content
                    {
                        Charset = "UTF-8",
                        Data = request.Body // Replace with your desired email content
                    }
                }
            }
        };

        try
        {
            await _emailService.SendEmailAsync(sendRequest, cancellationToken);
            request.Context.Logger.Log("Email sent");
        }
        catch (Exception ex)
        {
            request.Context.Logger.Log(ex.Message);
        }
    }

    public async Task SendEmail(CancellationToken cancellationToken)
    {
        
    }
}