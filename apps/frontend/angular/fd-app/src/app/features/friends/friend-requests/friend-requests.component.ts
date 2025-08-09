import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Store } from '@ngrx/store';
import { Observable, combineLatest, map } from 'rxjs';
import { FriendsStateSelectors } from 'src/app/state/friends';
import { FriendRequestDto, FriendRequestsDto } from '../../../dtos/friend.dto';
import { FriendsService } from '../../../services/friends.service';

@Component({
  selector: 'app-friend-requests',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './friend-requests.component.html',
})
export class FriendRequestsComponent {
  friendRequests$: Observable<FriendRequestsDto>;
  incomingRequests$: Observable<FriendRequestDto[]>;
  outgoingRequests$: Observable<FriendRequestDto[]>;
  hasOutgoingRequests$: Observable<boolean>;
  hasIncomingRequests$: Observable<boolean>;
  hasAnyRequests$: Observable<boolean>;
  processingRequest: string | null = null;

  constructor(private friendsService: FriendsService, private store: Store) {
    this.friendRequests$ = this.store.select(FriendsStateSelectors.selectFriendsStateFriendRequests);

    this.incomingRequests$ = this.friendRequests$.pipe(map(requests => requests?.IncomingRequests ?? []));

    this.outgoingRequests$ = this.friendRequests$.pipe(map(requests => requests?.OutgoingRequests ?? []));

    this.hasIncomingRequests$ = this.incomingRequests$.pipe(map(requests => requests.length > 0));

    this.hasOutgoingRequests$ = this.outgoingRequests$.pipe(map(requests => requests.length > 0));

    this.hasAnyRequests$ = combineLatest([this.hasIncomingRequests$, this.hasOutgoingRequests$]).pipe(
      map(([hasIncoming, hasOutgoing]) => hasIncoming || hasOutgoing)
    );
  }

  trackByRequestId(index: number, request: FriendRequestDto): string {
    return request.RequestId;
  }

  acceptRequest(request: FriendRequestDto): void {
    this.processingRequest = request.RequestId;

    this.friendsService.acceptFriendRequest(request.RequestId).subscribe({
      next: () => {
        console.log('Friend request accepted successfully');
        this.processingRequest = null;
      },
      error: error => {
        console.error('Error accepting friend request:', error);
        this.processingRequest = null;
      },
    });
  }

  declineRequest(request: FriendRequestDto): void {
    this.processingRequest = request.RequestId;

    this.friendsService.declineFriendRequest(request.RequestId).subscribe({
      next: () => {
        console.log('Friend request declined successfully');
        this.processingRequest = null;
      },
      error: error => {
        console.error('Error declining friend request:', error);
        this.processingRequest = null;
      },
    });
  }
}
