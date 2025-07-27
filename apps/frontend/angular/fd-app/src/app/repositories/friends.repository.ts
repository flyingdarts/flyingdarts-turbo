import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { environment } from "src/environments/environment";
import {
  FriendDto,
  FriendRequestDto,
  FriendRequestResponseDto,
  UserSearchDto,
} from "../dtos/friend.dto";

@Injectable({ providedIn: "root" })
export class FriendsRepository {
  private baseHref = "";

  constructor(private httpClient: HttpClient) {
    this.baseHref = environment.friendsApi;
  }

  // Get all friends
  public getFriends(): Observable<FriendDto[]> {
    return this.httpClient.get<FriendDto[]>(`${this.baseHref}`);
  }

  // Get friend requests
  public getFriendRequests(): Observable<FriendRequestResponseDto> {
    return this.httpClient.get<FriendRequestResponseDto>(
      `${this.baseHref}/requests`
    );
  }

  // Send friend request
  public sendFriendRequest(
    targetUserId: string,
    message?: string
  ): Observable<any> {
    return this.httpClient.post(`${this.baseHref}/request`, {
      TargetUserId: targetUserId,
      Message: message,
    });
  }

  // Accept friend request
  public acceptFriendRequest(requestId: string): Observable<any> {
    return this.httpClient.put(
      `${this.baseHref}/request/${encodeURIComponent(requestId)}`,
      {
        Accept: true,
      }
    );
  }

  // Decline friend request
  public declineFriendRequest(requestId: string): Observable<any> {
    return this.httpClient.put(
      `${this.baseHref}/request/${encodeURIComponent(requestId)}`,
      {
        Accept: false,
      }
    );
  }

  // Cancel friend request
  public cancelFriendRequest(requestId: string): Observable<any> {
    return this.httpClient.delete(`${this.baseHref}/requests/${requestId}`);
  }

  // Remove friend
  public removeFriend(friendId: string): Observable<any> {
    return this.httpClient.delete(`${this.baseHref}/${friendId}`);
  }

  // Search users
  public searchUsers(query: string): Observable<UserSearchDto[]> {
    return this.httpClient.post<UserSearchDto[]>(`${this.baseHref}/search`, {
      SearchTerm: query,
    });
  }
}
