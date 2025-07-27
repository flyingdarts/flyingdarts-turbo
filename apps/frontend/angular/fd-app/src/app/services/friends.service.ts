import { Injectable } from "@angular/core";
import { BehaviorSubject, Observable, tap } from "rxjs";
import {
  FriendDto,
  FriendRequestDto,
  FriendRequestResponseDto,
  UserSearchDto,
} from "../dtos/friend.dto";
import { FriendsRepository } from "../repositories/friends.repository";

@Injectable({ providedIn: "root" })
export class FriendsService {
  private friendsSubject = new BehaviorSubject<FriendDto[]>([]);
  private friendRequestsSubject = new BehaviorSubject<FriendRequestResponseDto>(
    {
      IncomingRequests: [],
      OutgoingRequests: [],
      any: false,
      length: 0,
    }
  );

  public friends$ = this.friendsSubject.asObservable();
  public friendRequests$ = this.friendRequestsSubject.asObservable();

  constructor(private friendsRepository: FriendsRepository) {}

  // Load friends
  public loadFriends(): Observable<FriendDto[]> {
    return this.friendsRepository
      .getFriends()
      .pipe(tap((friends) => this.friendsSubject.next(friends)));
  }

  // Load friend requests
  public loadFriendRequests(): Observable<FriendRequestResponseDto> {
    return this.friendsRepository.getFriendRequests().pipe(
      tap((requests) => {
        console.log(requests);
        return this.friendRequestsSubject.next(requests);
      })
    );
  }

  // Send friend request
  public sendFriendRequest(
    targetUserId: string,
    message?: string
  ): Observable<any> {
    return this.friendsRepository.sendFriendRequest(targetUserId, message).pipe(
      tap(() => {
        // Reload friend requests after sending
        this.loadFriendRequests().subscribe();
      })
    );
  }

  // Accept friend request
  public acceptFriendRequest(requestId: string): Observable<any> {
    return this.friendsRepository.acceptFriendRequest(requestId).pipe(
      tap(() => {
        // Reload both friends and requests after accepting
        this.loadFriends().subscribe();
        this.loadFriendRequests().subscribe();
      })
    );
  }

  // Decline friend request
  public declineFriendRequest(requestId: string): Observable<any> {
    return this.friendsRepository.declineFriendRequest(requestId).pipe(
      tap(() => {
        // Reload friend requests after declining
        this.loadFriendRequests().subscribe();
      })
    );
  }

  // Cancel friend request
  public cancelFriendRequest(requestId: string): Observable<any> {
    return this.friendsRepository.cancelFriendRequest(requestId).pipe(
      tap(() => {
        // Reload friend requests after cancelling
        this.loadFriendRequests().subscribe();
      })
    );
  }

  // Remove friend
  public removeFriend(friendId: string): Observable<any> {
    return this.friendsRepository.removeFriend(friendId).pipe(
      tap(() => {
        // Reload friends after removing
        this.loadFriends().subscribe();
      })
    );
  }

  // Search users
  public searchUsers(query: string): Observable<UserSearchDto[]> {
    return this.friendsRepository.searchUsers(query);
  }

  // Get current friends
  public getCurrentFriends(): FriendDto[] {
    return this.friendsSubject.value;
  }

  // Get current friend requests
  public getCurrentFriendRequests(): FriendRequestResponseDto {
    return this.friendRequestsSubject.value;
  }
}
