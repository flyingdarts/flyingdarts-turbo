﻿using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;
using Authress.SDK;
using Authress.SDK.Client;
using Flyingdarts.Backend.Auth;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

// Import statements are organized and simplified

// Create a serializer for JSON serialization and deserialization
var serializer = new DefaultLambdaJsonSerializer(x => x.PropertyNameCaseInsensitive = true);

// Define the Lambda function handler
var handler = async (APIGatewayCustomAuthorizerRequest apiGatewayEvent, ILambdaContext context) =>
{
    string ExtractToken()
    {
        return apiGatewayEvent.QueryStringParameters.ContainsKey("token") 
            ? apiGatewayEvent.QueryStringParameters["token"] 
            : apiGatewayEvent.Headers["Authorization"];
    }

    async Task<string> ValidateToken(string token)
    {
        var authressSettings = new AuthressSettings { ApiBasePath = Environment.GetEnvironmentVariable("AuthressApiBasePath") };
        var tokenProvider = new ManualTokenProvider();
        tokenProvider.SetToken(token);
        var authressClient = new AuthressClient(tokenProvider, authressSettings);
        var authressIdentity = await authressClient.VerifyToken(token);
        return authressIdentity.UserId;
    }
    
    try
    {
        var userId = await ValidateToken(ExtractToken());

        return new APIGatewayCustomAuthorizerResponse
        {
            PrincipalID =  userId,
            PolicyDocument = new APIGatewayCustomAuthorizerPolicy
            {
                Statement = new List<APIGatewayCustomAuthorizerPolicy.IAMPolicyStatement>
                {
                    new()
                    {
                        Effect = "Allow",
                        Resource = new HashSet<string> { apiGatewayEvent.MethodArn },
                        Action = new HashSet<string> { "execute-api:Invoke" }
                    }
                }
            },
            Context = new APIGatewayCustomAuthorizerContextOutput
            {
                { "UserId", userId }
            }
        };
    }
    catch
    {
        return new APIGatewayCustomAuthorizerResponse
        {
            PrincipalID = "401",
            PolicyDocument = new APIGatewayCustomAuthorizerPolicy
            {
                Statement = new List<APIGatewayCustomAuthorizerPolicy.IAMPolicyStatement>
                {
                    new()
                    {
                        Effect = "Deny",
                        Resource = new HashSet<string> { apiGatewayEvent.MethodArn },
                        Action = new HashSet<string> { "execute-api:Invoke" }
                    }
                }
            }
        };
    }
};

// Create and run the Lambda function (kept as per your original structure)
await LambdaBootstrapBuilder.Create(handler, serializer)
    .Build()
    .RunAsync();