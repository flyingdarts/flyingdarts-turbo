// Import statements are organized and simplified

// Create a serializer for JSON serialization and deserialization

using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;
using Authress.SDK;
using Authress.SDK.Client;

var serializer = new DefaultLambdaJsonSerializer(x => x.PropertyNameCaseInsensitive = true);

// Define the Lambda function handler
var handler = async (APIGatewayCustomAuthorizerRequest apiGatewayEvent, ILambdaContext context) =>
{
    Console.WriteLine($"[AUTH] Starting authorization for request: {apiGatewayEvent.MethodArn}");
    Console.WriteLine(
        $"[AUTH] Request context: ConnectionId={apiGatewayEvent.RequestContext?.ConnectionId}, RouteKey={apiGatewayEvent.RequestContext?.RouteKey}"
    );
    Console.WriteLine($"[AUTH] Headers count: {apiGatewayEvent.Headers?.Count ?? 0}");
    Console.WriteLine(
        $"[AUTH] Query string parameters count: {apiGatewayEvent.QueryStringParameters?.Count ?? 0}"
    );

    if (apiGatewayEvent.Headers != null)
    {
        foreach (var header in apiGatewayEvent.Headers)
        {
            Console.WriteLine($"[AUTH] Header: {header.Key} = {header.Value}");
        }
    }

    if (apiGatewayEvent.QueryStringParameters != null)
    {
        foreach (var param in apiGatewayEvent.QueryStringParameters)
        {
            Console.WriteLine($"[AUTH] Query param: {param.Key} = {param.Value}");
        }
    }

    string? ExtractToken()
    {
        Console.WriteLine($"[AUTH] Extracting token...");
        Console.WriteLine($"[AUTH] ConnectionId: '{apiGatewayEvent.RequestContext?.ConnectionId}'");

        if (string.IsNullOrEmpty(apiGatewayEvent.RequestContext?.ConnectionId))
        {
            Console.WriteLine(
                $"[AUTH] No ConnectionId found, extracting from Authorization header"
            );
            var authHeader =
                apiGatewayEvent.Headers?.ContainsKey("Authorization") == true
                    ? apiGatewayEvent.Headers["Authorization"]
                    : null;
            Console.WriteLine($"[AUTH] Authorization header value: '{authHeader}'");

            if (string.IsNullOrEmpty(authHeader))
            {
                Console.WriteLine($"[AUTH] WARNING: Authorization header is null or empty");
                return null;
            }

            // Remove "Bearer " prefix if present
            var token = authHeader.StartsWith("Bearer ") ? authHeader.Substring(7) : authHeader;
            Console.WriteLine($"[AUTH] Extracted token (without Bearer prefix): '{token}'");
            return token;
        }

        Console.WriteLine(
            $"[AUTH] ConnectionId found, extracting from query string token parameter"
        );
        var queryToken =
            apiGatewayEvent.QueryStringParameters?.ContainsKey("token") == true
                ? apiGatewayEvent.QueryStringParameters["token"]
                : null;

        Console.WriteLine($"[AUTH] Query string token value: '{queryToken}'");

        if (string.IsNullOrEmpty(queryToken))
        {
            Console.WriteLine($"[AUTH] WARNING: Query string token parameter is null or empty");
            return null;
        }

        return queryToken;
    }

    async Task<string> ValidateToken(string token)
    {
        Console.WriteLine($"[AUTH] Starting token validation...");
        Console.WriteLine($"[AUTH] Token to validate: '{token}'");

        if (string.IsNullOrEmpty(token))
        {
            Console.WriteLine($"[AUTH] ERROR: Token is null or empty");
            throw new ArgumentException("Token cannot be null or empty");
        }

        try
        {
            var authressApiUrl = Environment.GetEnvironmentVariable("AuthressApiBasePath");
            Console.WriteLine($"[AUTH] Authress API URL: '{authressApiUrl}'");

            if (string.IsNullOrEmpty(authressApiUrl))
            {
                Console.WriteLine($"[AUTH] ERROR: AuthressApiUrl environment variable is not set");
                throw new InvalidOperationException(
                    "AuthressApiUrl environment variable is not set"
                );
            }

            var authressSettings = new AuthressSettings { AuthressApiUrl = authressApiUrl };

            Console.WriteLine(
                $"[AUTH] Created AuthressSettings with URL: {authressSettings.AuthressApiUrl}"
            );

            var tokenProvider = new ManualTokenProvider();
            tokenProvider.SetToken(token);
            Console.WriteLine($"[AUTH] Created ManualTokenProvider and set token");

            var authressClient = new AuthressClient(tokenProvider, authressSettings);
            Console.WriteLine($"[AUTH] Created AuthressClient");

            Console.WriteLine($"[AUTH] Calling AuthressClient.VerifyToken...");
            var authressIdentity = await authressClient.VerifyToken(token);
            Console.WriteLine(
                $"[AUTH] Token verification successful. UserId: '{authressIdentity.UserId}'"
            );

            return authressIdentity.UserId;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AUTH] ERROR during token validation: {ex.Message}");
            Console.WriteLine($"[AUTH] Exception type: {ex.GetType().Name}");
            Console.WriteLine($"[AUTH] Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    try
    {
        Console.WriteLine($"[AUTH] Starting authorization process...");

        var token = ExtractToken();
        if (string.IsNullOrEmpty(token))
        {
            Console.WriteLine($"[AUTH] ERROR: Failed to extract token");
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
                            Action = new HashSet<string> { "execute-api:Invoke" },
                        },
                    },
                },
            };
        }

        Console.WriteLine($"[AUTH] Token extracted successfully, proceeding to validation");
        var userId = await ValidateToken(token);
        var connectionId = apiGatewayEvent.RequestContext?.ConnectionId;

        Console.WriteLine(
            $"[AUTH] Authorization successful. UserId: '{userId}', ConnectionId: '{connectionId}'"
        );
        Console.WriteLine($"[AUTH] MethodArn: '{apiGatewayEvent.MethodArn}'");

        return new APIGatewayCustomAuthorizerResponse
        {
            PrincipalID = userId,
            PolicyDocument = new APIGatewayCustomAuthorizerPolicy
            {
                Statement = new List<APIGatewayCustomAuthorizerPolicy.IAMPolicyStatement>
                {
                    new()
                    {
                        Effect = "Allow",
                        Resource = new HashSet<string> { "*" },
                        Action = new HashSet<string> { "execute-api:Invoke" },
                    },
                },
            },
            Context = new APIGatewayCustomAuthorizerContextOutput
            {
                { "UserId", userId },
                { "ConnectionId", connectionId ?? string.Empty },
            },
        };
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[AUTH] ERROR during authorization: {ex.Message}");
        Console.WriteLine($"[AUTH] Exception type: {ex.GetType().Name}");
        Console.WriteLine($"[AUTH] Stack trace: {ex.StackTrace}");
        Console.WriteLine($"[AUTH] Returning 401 response");

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
                        Action = new HashSet<string> { "execute-api:Invoke" },
                    },
                },
            },
        };
    }
};

// Create and run the Lambda function (kept as per your original structure)
await LambdaBootstrapBuilder.Create(handler, serializer).Build().RunAsync();
