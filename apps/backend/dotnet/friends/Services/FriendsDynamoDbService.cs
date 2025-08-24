namespace Flyingdarts.Backend.Friends.Api.Services;

public interface IFriendsDynamoDbService : IDynamoDbService
{
    Task<bool> CheckUserExistsAsync(string userId, CancellationToken cancellationToken);
    Task<bool> CheckIfAlreadyFriendsAsync(
        string userId1,
        string userId2,
        CancellationToken cancellationToken
    );
    Task<bool> CheckExistingRequestAsync(
        string requesterId,
        string targetUserId,
        CancellationToken cancellationToken
    );
    Task<FriendRequest?> GetFriendRequestAsync(
        string requestId,
        CancellationToken cancellationToken
    );
    Task<List<FriendRequest>> GetIncomingFriendRequestsAsync(
        string userId,
        CancellationToken cancellationToken
    );
    Task<List<FriendRequest>> GetOutgoingFriendRequestsAsync(
        string userId,
        CancellationToken cancellationToken
    );
    Task<List<FriendRelationship>> GetUserFriendsAsync(
        string userId,
        CancellationToken cancellationToken
    );
    Task<FriendRelationship?> GetFriendRelationshipAsync(
        string userId,
        string friendId,
        CancellationToken cancellationToken
    );
    Task<List<User>> SearchUsersAsync(
        string searchTerm,
        string authUserId,
        CancellationToken cancellationToken
    );
    Task SaveFriendRequestAsync(FriendRequest friendRequest, CancellationToken cancellationToken);
    Task AcceptFriendRequestAsync(FriendRequest friendRequest, CancellationToken cancellationToken);
    Task DeclineFriendRequestAsync(
        FriendRequest friendRequest,
        CancellationToken cancellationToken
    );
    Task DeleteFriendRelationshipsAsync(
        FriendRelationship? relationship1,
        FriendRelationship? relationship2,
        CancellationToken cancellationToken
    );
}

public class FriendsDynamoDbService : DynamoDbService, IFriendsDynamoDbService
{
    private readonly IDynamoDBContext _dbContext;
    private readonly IOptions<ApplicationOptions> _options;

    public FriendsDynamoDbService(IDynamoDBContext dbContext, IOptions<ApplicationOptions> options)
        : base(dbContext, options)
    {
        _dbContext = dbContext;
        _options = options;
    }

    public async Task<bool> CheckUserExistsAsync(string userId, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[FriendsDynamoDbService] Checking if user exists: {userId}");

        try
        {
            var user = await ReadUserAsync(userId, cancellationToken);
            return user is not null;
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"[FriendsDynamoDbService] Error checking if user {userId} exists: {ex.Message}"
            );
            return false;
        }
    }

    public async Task<bool> CheckIfAlreadyFriendsAsync(
        string userId1,
        string userId2,
        CancellationToken cancellationToken
    )
    {
        Console.WriteLine(
            $"[FriendsDynamoDbService] Checking if users are already friends: {userId1} and {userId2}"
        );

        try
        {
            var queryFilter = new QueryFilter("PK", QueryOperator.Equal, "FRIEND#RELATIONSHIP");
            queryFilter.AddCondition("SK", QueryOperator.Equal, $"{userId1}#{userId2}");

            var queryConfig = new QueryOperationConfig { Filter = queryFilter };

            var results = await _dbContext
                .FromQueryAsyncCompat<FriendRelationship>(
                    queryConfig,
                    _options.Value.ToOperationConfig()
                )
                .GetRemainingAsync(cancellationToken);

            var areFriends = results.Any(f => f.Status == FriendshipStatus.Accepted);
            Console.WriteLine(
                $"[FriendsDynamoDbService] Users {userId1} and {userId2} are friends: {areFriends}"
            );
            return areFriends;
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"[FriendsDynamoDbService] Error checking friendship between {userId1} and {userId2}: {ex.Message}"
            );
            return false;
        }
    }

    public async Task<bool> CheckExistingRequestAsync(
        string requesterId,
        string targetUserId,
        CancellationToken cancellationToken
    )
    {
        Console.WriteLine(
            $"[FriendsDynamoDbService] Checking if friend request exists from {requesterId} to {targetUserId}"
        );

        try
        {
            var queryFilter = new QueryFilter("PK", QueryOperator.Equal, Constants.FriendRequest);
            queryFilter.AddCondition(
                "SK",
                QueryOperator.BeginsWith,
                $"{targetUserId}#{requesterId}#"
            );

            var queryConfig = new QueryOperationConfig { Filter = queryFilter };

            var results = await _dbContext
                .FromQueryAsyncCompat<FriendRequest>(
                    queryConfig,
                    _options.Value.ToOperationConfig()
                )
                .GetRemainingAsync(cancellationToken);

            var hasExistingRequest = results.Any(r => r.Status == FriendRequestStatus.Pending);
            Console.WriteLine(
                $"[FriendsDynamoDbService] Friend request exists from {requesterId} to {targetUserId}: {hasExistingRequest}"
            );
            return hasExistingRequest;
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"[FriendsDynamoDbService] Error checking existing request from {requesterId} to {targetUserId}: {ex.Message}"
            );
            return false;
        }
    }

    public async Task<FriendRequest?> GetFriendRequestAsync(
        string requestId,
        CancellationToken cancellationToken
    )
    {
        Console.WriteLine($"[FriendsDynamoDbService] Getting friend request: {requestId}");

        try
        {
            var queryFilter = new QueryFilter("PK", QueryOperator.Equal, "FRIEND#REQUEST");
            queryFilter.AddCondition("SK", QueryOperator.Equal, requestId);

            var queryConfig = new QueryOperationConfig { Filter = queryFilter };

            var results = await _dbContext
                .FromQueryAsyncCompat<FriendRequest>(
                    queryConfig,
                    _options.Value.ToOperationConfig()
                )
                .GetRemainingAsync(cancellationToken);

            var friendRequest = results.FirstOrDefault();
            Console.WriteLine(
                $"[FriendsDynamoDbService] Friend request {requestId} found: {friendRequest != null}"
            );
            return friendRequest;
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"[FriendsDynamoDbService] Error getting friend request {requestId}: {ex.Message}"
            );
            return null;
        }
    }

    public async Task<List<FriendRequest>> GetIncomingFriendRequestsAsync(
        string userId,
        CancellationToken cancellationToken
    )
    {
        Console.WriteLine(
            $"[FriendsDynamoDbService] Getting incoming friend requests for user: {userId}"
        );

        try
        {
            var queryFilter = new QueryFilter("PK", QueryOperator.Equal, "FRIEND#REQUEST");
            queryFilter.AddCondition("SK", QueryOperator.BeginsWith, $"{userId}#");

            var queryConfig = new QueryOperationConfig { Filter = queryFilter };

            var results = await _dbContext
                .FromQueryAsyncCompat<FriendRequest>(
                    queryConfig,
                    _options.Value.ToOperationConfig()
                )
                .GetRemainingAsync(cancellationToken);

            var incomingRequests = results
                .Where(r => r.Status == FriendRequestStatus.Pending && r.TargetUserId == userId)
                .ToList();

            Console.WriteLine(
                $"[FriendsDynamoDbService] Found {incomingRequests.Count} incoming friend requests for user {userId}"
            );
            return incomingRequests;
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"[FriendsDynamoDbService] Error getting incoming friend requests for user {userId}: {ex.Message}"
            );
            return new List<FriendRequest>();
        }
    }

    public async Task<List<FriendRequest>> GetOutgoingFriendRequestsAsync(
        string userId,
        CancellationToken cancellationToken
    )
    {
        Console.WriteLine(
            $"[FriendsDynamoDbService] Getting outgoing friend requests for user: {userId}"
        );

        try
        {
            // For outgoing requests, we need to scan since the requester is not in the primary key
            var scanConfig = new ScanOperationConfig();

            var results = await _dbContext
                .FromScanAsyncCompat<FriendRequest>(scanConfig, _options.Value.ToOperationConfig())
                .GetRemainingAsync(cancellationToken);

            var outgoingRequests = results
                .Where(r => r.Status == FriendRequestStatus.Pending && r.RequesterId == userId)
                .ToList();

            Console.WriteLine(
                $"[FriendsDynamoDbService] Found {outgoingRequests.Count} outgoing friend requests for user {userId}"
            );
            return outgoingRequests;
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"[FriendsDynamoDbService] Error getting outgoing friend requests for user {userId}: {ex.Message}"
            );
            return new List<FriendRequest>();
        }
    }

    public async Task<List<FriendRelationship>> GetUserFriendsAsync(
        string userId,
        CancellationToken cancellationToken
    )
    {
        Console.WriteLine($"[FriendsDynamoDbService] Getting friends for user: {userId}");

        try
        {
            var queryFilter = new QueryFilter("PK", QueryOperator.Equal, "FRIEND#RELATIONSHIP");
            queryFilter.AddCondition("SK", QueryOperator.BeginsWith, $"{userId}#");

            var queryConfig = new QueryOperationConfig { Filter = queryFilter };

            var results = await _dbContext
                .FromQueryAsyncCompat<FriendRelationship>(
                    queryConfig,
                    _options.Value.ToOperationConfig()
                )
                .GetRemainingAsync(cancellationToken);

            var friends = results.Where(f => f.Status == FriendshipStatus.Accepted).ToList();
            Console.WriteLine(
                $"[FriendsDynamoDbService] Found {friends.Count} friends for user {userId}"
            );
            return friends;
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"[FriendsDynamoDbService] Error getting friends for user {userId}: {ex.Message}"
            );
            return new List<FriendRelationship>();
        }
    }

    public async Task<FriendRelationship?> GetFriendRelationshipAsync(
        string userId,
        string friendId,
        CancellationToken cancellationToken
    )
    {
        Console.WriteLine(
            $"[FriendsDynamoDbService] Getting friend relationship between {userId} and {friendId}"
        );

        try
        {
            var queryFilter = new QueryFilter("PK", QueryOperator.Equal, "FRIEND#RELATIONSHIP");
            queryFilter.AddCondition("SK", QueryOperator.Equal, $"{userId}#{friendId}");

            var queryConfig = new QueryOperationConfig { Filter = queryFilter };

            var results = await _dbContext
                .FromQueryAsyncCompat<FriendRelationship>(
                    queryConfig,
                    _options.Value.ToOperationConfig()
                )
                .GetRemainingAsync(cancellationToken);

            var relationship = results.FirstOrDefault(f => f.Status == FriendshipStatus.Accepted);
            Console.WriteLine(
                $"[FriendsDynamoDbService] Friend relationship found between {userId} and {friendId}: {relationship != null}"
            );
            return relationship;
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"[FriendsDynamoDbService] Error getting friend relationship between {userId} and {friendId}: {ex.Message}"
            );
            return null;
        }
    }

    public async Task<List<User>> SearchUsersAsync(
        string searchTerm,
        string authUserId,
        CancellationToken cancellationToken
    )
    {
        Console.WriteLine($"[FriendsDynamoDbService] Searching users with term: '{searchTerm}'");

        try
        {
            // For now, we'll do a scan since DynamoDB doesn't have great text search
            // In production, you might want to use ElasticSearch or other search service
            var scanConfig = new ScanOperationConfig();

            var results = await _dbContext
                .FromScanAsyncCompat<User>(scanConfig, _options.Value.ToOperationConfig())
                .GetRemainingAsync(cancellationToken);

            Console.WriteLine(
                $"[FriendsDynamoDbService] Database scan completed. Found {results.Count()} total users"
            );

            // Log users with null profiles for debugging
            var usersWithNullProfiles = results.Where(u => u.Profile == null).ToList();
            if (usersWithNullProfiles.Any())
            {
                Console.WriteLine(
                    $"[FriendsDynamoDbService] Found {usersWithNullProfiles.Count} users with null profiles: {string.Join(", ", usersWithNullProfiles.Select(u => u.UserId))}"
                );
            }

            // Filter results based on username or email containing the search term
            var filteredUsers = results
                .Where(u =>
                    (
                        u.Profile?.UserName?.Contains(
                            searchTerm,
                            StringComparison.OrdinalIgnoreCase
                        ) == true
                    )
                    || (
                        u.Profile?.Email?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                        == true
                    )
                    || u.UserId == searchTerm
                )
                .Where(u => u.UserId != authUserId)
                .Take(10)
                .ToList(); // Limit to 10 results

            Console.WriteLine(
                $"[FriendsDynamoDbService] Found {filteredUsers.Count} users matching search term '{searchTerm}'"
            );
            return filteredUsers;
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"[FriendsDynamoDbService] Error searching users with term '{searchTerm}': {ex.Message}"
            );
            Console.WriteLine($"[FriendsDynamoDbService] Exception type: {ex.GetType().Name}");
            Console.WriteLine($"[FriendsDynamoDbService] Stack trace: {ex.StackTrace}");
            return new List<User>();
        }
    }

    public async Task SaveFriendRequestAsync(
        FriendRequest friendRequest,
        CancellationToken cancellationToken
    )
    {
        Console.WriteLine(
            $"[FriendsDynamoDbService] Saving friend request from {friendRequest.RequesterId} to {friendRequest.TargetUserId}"
        );

        try
        {
            var batch = _dbContext.CreateBatchWriteCompat<FriendRequest>(
                _options.Value.ToOperationConfig()
            );
            batch.AddPutItem(friendRequest);
            await batch.ExecuteAsync(cancellationToken);
            Console.WriteLine(
                $"[FriendsDynamoDbService] Successfully saved friend request from {friendRequest.RequesterId} to {friendRequest.TargetUserId}"
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"[FriendsDynamoDbService] Error saving friend request from {friendRequest.RequesterId} to {friendRequest.TargetUserId}: {ex.Message}"
            );
            throw;
        }
    }

    public async Task AcceptFriendRequestAsync(
        FriendRequest friendRequest,
        CancellationToken cancellationToken
    )
    {
        Console.WriteLine(
            $"[FriendsDynamoDbService] Accepting friend request from {friendRequest.RequesterId} to {friendRequest.TargetUserId}"
        );

        try
        {
            var now = DateTime.UtcNow;

            // Update the friend request status
            friendRequest.Status = FriendRequestStatus.Accepted;

            // Create bidirectional friend relationships
            var relationship1 = FriendRelationship.Create(
                friendRequest.RequesterId,
                friendRequest.TargetUserId,
                friendRequest.CreatedAt,
                friendRequest.Message
            );

            var relationship2 = FriendRelationship.Create(
                friendRequest.TargetUserId,
                friendRequest.RequesterId,
                friendRequest.CreatedAt,
                friendRequest.Message
            );

            // Save all changes
            var requestBatch = _dbContext.CreateBatchWriteCompat<FriendRequest>(
                _options.Value.ToOperationConfig()
            );
            requestBatch.AddPutItem(friendRequest);

            var relationshipBatch = _dbContext.CreateBatchWriteCompat<FriendRelationship>(
                _options.Value.ToOperationConfig()
            );
            relationshipBatch.AddPutItem(relationship1);
            relationshipBatch.AddPutItem(relationship2);

            await requestBatch.ExecuteAsync(cancellationToken);
            await relationshipBatch.ExecuteAsync(cancellationToken);

            Console.WriteLine(
                $"[FriendsDynamoDbService] Successfully accepted friend request from {friendRequest.RequesterId} to {friendRequest.TargetUserId}"
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"[FriendsDynamoDbService] Error accepting friend request from {friendRequest.RequesterId} to {friendRequest.TargetUserId}: {ex.Message}"
            );
            throw;
        }
    }

    public async Task DeclineFriendRequestAsync(
        FriendRequest friendRequest,
        CancellationToken cancellationToken
    )
    {
        Console.WriteLine(
            $"[FriendsDynamoDbService] Declining friend request from {friendRequest.RequesterId} to {friendRequest.TargetUserId}"
        );

        try
        {
            // Update the friend request status
            friendRequest.Status = FriendRequestStatus.Declined;

            var batch = _dbContext.CreateBatchWriteCompat<FriendRequest>(
                _options.Value.ToOperationConfig()
            );
            batch.AddPutItem(friendRequest);
            await batch.ExecuteAsync(cancellationToken);

            Console.WriteLine(
                $"[FriendsDynamoDbService] Successfully declined friend request from {friendRequest.RequesterId} to {friendRequest.TargetUserId}"
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"[FriendsDynamoDbService] Error declining friend request from {friendRequest.RequesterId} to {friendRequest.TargetUserId}: {ex.Message}"
            );
            throw;
        }
    }

    public async Task DeleteFriendRelationshipsAsync(
        FriendRelationship? relationship1,
        FriendRelationship? relationship2,
        CancellationToken cancellationToken
    )
    {
        var relationship1Info =
            relationship1 != null
                ? $"{relationship1.RequesterId}-{relationship1.FriendId}"
                : "null";
        var relationship2Info =
            relationship2 != null
                ? $"{relationship2.RequesterId}-{relationship2.FriendId}"
                : "null";
        Console.WriteLine(
            $"[FriendsDynamoDbService] Deleting friend relationships: {relationship1Info} and {relationship2Info}"
        );

        try
        {
            var batch = _dbContext.CreateBatchWriteCompat<FriendRelationship>(
                _options.Value.ToOperationConfig()
            );
            var hasItemsToDelete = false;

            if (relationship1 != null)
            {
                batch.AddDeleteItem(relationship1);
                hasItemsToDelete = true;
            }

            if (relationship2 != null)
            {
                batch.AddDeleteItem(relationship2);
                hasItemsToDelete = true;
            }

            if (hasItemsToDelete)
            {
                await batch.ExecuteAsync(cancellationToken);
                Console.WriteLine(
                    $"[FriendsDynamoDbService] Successfully deleted friend relationships: {relationship1Info} and {relationship2Info}"
                );
            }
            else
            {
                Console.WriteLine($"[FriendsDynamoDbService] No friend relationships to delete");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"[FriendsDynamoDbService] Error deleting friend relationships {relationship1Info} and {relationship2Info}: {ex.Message}"
            );
            throw;
        }
    }
}
