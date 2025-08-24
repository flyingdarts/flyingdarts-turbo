using System;

namespace Flyingdarts.Backend.Friends.Api.Models;

/// <summary>
/// Represents a friend of the current user.
/// </summary>
public class FriendDto
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public DateTime FriendsSince { get; set; }
    public bool IsOnline { get; set; }
    public string? ConnectionId { get; set; }
    public string? Picture { get; set; }
    public string? OpenGameId { get; set; }
}

/// <summary>
/// Represents a friend request sent by the current user.
/// </summary>
public class FriendRequestDto
{
    public string RequestId { get; set; } = string.Empty;
    public string RequesterId { get; set; } = string.Empty;
    public string RequesterUserName { get; set; } = string.Empty;
    public string TargetUserId { get; set; } = string.Empty;
    public string? Message { get; set; }
    public DateTime CreatedAt { get; set; }
    public FriendRequestStatus Status { get; set; }
}

/// <summary>
/// Represents a user search result.
/// </summary>
public class UserSearchDto
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public bool IsAlreadyFriend { get; set; }
    public bool HasPendingRequest { get; set; }
    public string? Picture { get; set; }
}
