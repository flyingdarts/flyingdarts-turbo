using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;
using Flyingdarts.DynamoDb.Service;
using Flyingdarts.Lambda.Core.Infrastructure;
using Flyingdarts.Meetings.Service.Services;
using Flyingdarts.Persistence;
using MediatR;

namespace Flyingdarts.Backend.Signalling.Api.Requests.Connect;

public class OnConnectCommandHandler : IRequestHandler<OnConnectCommand, APIGatewayProxyResponse>
{
    private readonly IMeetingService _meetingService;
    private readonly IDynamoDbService _dynamoDbService;

    public OnConnectCommandHandler(IMeetingService meetingService, IDynamoDbService dynamoDbService)
    {
        _meetingService = meetingService;
        _dynamoDbService = dynamoDbService;
    }

    public async Task<APIGatewayProxyResponse> Handle(
        OnConnectCommand request,
        CancellationToken cancellationToken
    )
    {
        Console.WriteLine($"[OnConnect] Processing request", JsonSerializer.Serialize(request));

        try
        {
            var user = await EnsureUserIsUpdatedOrCreated(request, cancellationToken);
            Console.WriteLine(
                $"[OnConnect] User ensured - UserId: {user.UserId}, AuthProviderUserId: {user.AuthProviderUserId}"
            );

            user.MeetingIdentifier = await EnsureUserHasMeetingRoom(user, cancellationToken);

            await _dynamoDbService.WriteUserAsync(user, cancellationToken);

            Console.WriteLine(
                $"[OnConnect] Meeting room ensured - MeetingIdentifier: {user.MeetingIdentifier}"
            );

            var body = new Dictionary<string, string>
            {
                { "UserId", user.UserId },
                { "MeetingIdentifier", user.MeetingIdentifier.ToString() },
            };

            Console.WriteLine(
                $"[OnConnect] Connection process completed successfully for UserId: {user.UserId}"
            );

            return ResponseBuilder.SuccessJson(body, 201);
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"[OnConnect] Error during connection process for AuthProviderUserId: {request.AuthProviderUserId}. Error: {ex.Message}"
            );
            Console.WriteLine($"[OnConnect] Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    private async Task<User> EnsureUserIsUpdatedOrCreated(
        OnConnectCommand request,
        CancellationToken cancellationToken
    )
    {
        Console.WriteLine(
            $"[OnConnect] Ensuring user exists for AuthProviderUserId: {request.AuthProviderUserId}"
        );

        User? user = null;
        try
        {
            Console.WriteLine($"[OnConnect] Attempting to read existing user from database");
            user = await _dynamoDbService.ReadUserByAuthProviderUserIdAsync(
                request.AuthProviderUserId,
                cancellationToken
            );
            Console.WriteLine($"[OnConnect] Existing user found - UserId: {user.UserId}");
        }
        catch (DynamoDbService.UserNotFoundException)
        {
            Console.WriteLine(
                $"[OnConnect] User not found, creating new user for AuthProviderUserId: {request.AuthProviderUserId}"
            );

            var userProfile = CreateFromAuthressToken(
                request.AuthressToken,
                request.IsServiceClient
            );
            Console.WriteLine(
                $"[OnConnect] User profile created from token - UserName: {userProfile.UserName}, Email: {userProfile.Email}"
            );

            user = User.Create(request.AuthProviderUserId, request.ConnectionId, userProfile);

            Console.WriteLine($"[OnConnect] New user created - UserId: {user.UserId}");
            await _dynamoDbService.WriteUserAsync(user, cancellationToken);
            Console.WriteLine($"[OnConnect] New user written to database successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"[OnConnect] Unexpected error while ensuring user for AuthProviderUserId: {request.AuthProviderUserId}. Error: {ex.Message}"
            );
            throw;
        }
        finally
        {
            if (user is null)
            {
                Console.WriteLine(
                    $"[OnConnect] User is null after all attempts for AuthProviderUserId: {request.AuthProviderUserId}"
                );
                throw new Exception("User not found");
            }
        }
        return user;
    }

    private UserProfile CreateFromAuthressToken(string token, bool isServiceClient = false)
    {
        Console.WriteLine("[OnConnect] Creating user profile from Authress token");

        try
        {
            Console.WriteLine($"[OnConnect] Token: {token}");

            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("[OnConnect] Token is null or empty");
                throw new Exception("Token is null or empty");
            }

            if (isServiceClient)
            {
                Console.WriteLine("[OnConnect] Creating service client user profile");
                return GetServiceClientUserProfile(token);
            }

            var normalizedToken = NormalizeAuthressToken(token);
            Console.WriteLine($"[OnConnect] Normalized token: {normalizedToken}");
            // Parse the JWT token to extract user information
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(normalizedToken);

            Console.WriteLine("[OnConnect] Parsing JWT token");
            var userProfile = new UserProfile();

            // Example: Extract "name" and "email" claims if they exist
            var nameClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "name");
            if (nameClaim != null)
            {
                userProfile.UserName = nameClaim.Value;
                Console.WriteLine($"[OnConnect] Extracted name from token: {nameClaim.Value}");
            }
            else
            {
                Console.WriteLine("[OnConnect] No 'name' claim found in token");
            }

            var emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "email");
            if (emailClaim != null)
            {
                userProfile.Email = emailClaim.Value;
                Console.WriteLine($"[OnConnect] Extracted email from token: {emailClaim.Value}");
            }
            else
            {
                Console.WriteLine("[OnConnect] No 'email' claim found in token");
            }

            var pictureClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "picture");
            if (pictureClaim != null)
            {
                userProfile.Picture = pictureClaim.Value;
                Console.WriteLine(
                    $"[OnConnect] Extracted picture from token: {pictureClaim.Value}"
                );
            }
            else
            {
                Console.WriteLine("[OnConnect] No 'picture' claim found in token");
            }

            Console.WriteLine(
                $"[OnConnect] User profile created successfully - UserName: {userProfile.UserName}, Email: {userProfile.Email}"
            );
            return userProfile;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[OnConnect] Error creating user profile from token: {ex.Message}");
            throw;
        }
    }

    private UserProfile GetServiceClientUserProfile(string token)
    {
        var idToken = token.Split('.')[1];
        var base64Decoded = Convert.FromBase64String(idToken);
        var jsonPayload = JsonSerializer.Deserialize<Dictionary<string, string>>(base64Decoded);

        return new UserProfile
        {
            UserName = jsonPayload["sub"],
            Email = "mike+test@flyingdarts.net",
            Country = "NL",
            Picture =
                "https://i.postimg.cc/HnD0HyQM/male-face-icon-default-profile-image-c3f2c592f9.jpg", // expires in 31 days
        };
    }

    private static string NormalizeAuthressToken(string authressToken)
    {
        if (string.IsNullOrWhiteSpace(authressToken))
        {
            return string.Empty;
        }

        // Normalize leading/trailing whitespace first
        var trimmedToken = authressToken.Trim();
        Console.WriteLine($"[OnConnect] Normalizing token: {trimmedToken}");

        const string userPrefix = "user=";

        // Case 1: Token starts with "user=" (case-insensitive)
        if (trimmedToken.StartsWith(userPrefix, StringComparison.OrdinalIgnoreCase))
        {
            var tokenValue = trimmedToken.Substring(userPrefix.Length).Trim();
            Console.WriteLine($"[OnConnect] Normalized token: {tokenValue}");
            return tokenValue;
        }

        Console.WriteLine($"[OnConnect] Token is not prefixed with 'user='");
        return trimmedToken;
    }

    private async Task<Guid> EnsureUserHasMeetingRoom(
        User user,
        CancellationToken cancellationToken
    )
    {
        Console.WriteLine($"[OnConnect] Ensuring user has meeting room for UserId: {user.UserId}");

        try
        {
            var meeting = await _meetingService.GetByNameAsync(user.UserId, cancellationToken);

            if (meeting is null)
            {
                Console.WriteLine(
                    $"[OnConnect] No existing meeting found for UserId: {user.UserId}, creating new meeting"
                );
                meeting = await _meetingService.CreateAsync(user.UserId, cancellationToken);

                if (meeting is null)
                {
                    Console.WriteLine(
                        $"[OnConnect] Failed to create meeting for UserId: {user.UserId}"
                    );
                    throw new Exception("Couldn't create or find a meeting for some reason");
                }

                if (meeting.Id is null)
                {
                    Console.WriteLine(
                        $"[OnConnect] Meeting created but ID is null for UserId: {user.UserId}. Meeting data: {JsonSerializer.Serialize(meeting)}"
                    );
                    throw new Exception(
                        $"Couldn't add the meeting because the id was null {JsonSerializer.Serialize(meeting)}"
                    );
                }

                Console.WriteLine(
                    $"[OnConnect] New meeting created successfully - MeetingId: {meeting.Id}"
                );
                await _dynamoDbService.WriteUserAsync(user, cancellationToken);
                Console.WriteLine($"[OnConnect] User updated with new meeting in database");
            }
            else
            {
                Console.WriteLine(
                    $"[OnConnect] Existing meeting found for UserId: {user.UserId} - MeetingId: {meeting.Id}"
                );
            }

            var meetingId = meeting.Id ?? throw new Exception("Cant add user id");
            Console.WriteLine(
                $"[OnConnect] Meeting room ensured successfully - MeetingId: {meetingId}"
            );
            return meetingId;
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"[OnConnect] Error ensuring meeting room for UserId: {user.UserId}. Error: {ex.Message}"
            );
            throw;
        }
    }
}
