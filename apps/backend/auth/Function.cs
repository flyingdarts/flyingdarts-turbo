using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;

// Create a serializer for JSON serialization and deserialization
var serializer = new DefaultLambdaJsonSerializer(x => x.PropertyNameCaseInsensitive = true);

// Define the Lambda function handler
var handler = async (APIGatewayCustomAuthorizerRequest apiGatewayEvent, ILambdaContext context) =>
{
    async Task<bool> ValidateToken(string token)
    {
        // Use the AWS Cognito SDK or any other authentication service to validate the token
        try
        {
            var request = new GetUserRequest
            {
                AccessToken = token,
            };

            var response = await new AmazonCognitoIdentityProviderClient().GetUserAsync(request);
            
            // If the call is successful, the token is valid
            return true;
        }
        catch (NotAuthorizedException ex)
        {
            Console.WriteLine(ex);
            return false;
        }
    }

    var token = apiGatewayEvent.QueryStringParameters["token"];
    var methodArn = System.Environment.GetEnvironmentVariable("ConnectMethodArn");

    return new APIGatewayCustomAuthorizerResponse()
    {
        PrincipalID = string.IsNullOrEmpty(token) || !await ValidateToken(token) ? "401" : "user",
        PolicyDocument = new APIGatewayCustomAuthorizerPolicy()
        {
            Statement = new List<APIGatewayCustomAuthorizerPolicy.IAMPolicyStatement>
            {
                new APIGatewayCustomAuthorizerPolicy.IAMPolicyStatement()
                {
                    Effect = string.IsNullOrEmpty(token) || !await ValidateToken(token) ? "Deny" : "Allow",
                    Resource = new HashSet<string> { methodArn },
                    Action = new HashSet<string> { "execute-api:Invoke" }
                }
            }
        }
    };
};

// Create and run the Lambda function
await LambdaBootstrapBuilder.Create(handler, serializer)
    .Build()
    .RunAsync();

