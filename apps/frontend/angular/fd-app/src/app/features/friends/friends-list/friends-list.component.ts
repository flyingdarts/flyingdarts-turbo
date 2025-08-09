import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs';
import { FriendsStateActions, FriendsStateSelectors } from 'src/app/state/friends';
import { FriendDto } from '../../../dtos/friend.dto';

@Component({
  selector: 'app-friends-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './friends-list.component.html',
})
export class FriendsListComponent {
  friends$: Observable<FriendDto[]>;

  constructor(private router: Router, private store: Store) {
    this.friends$ = this.store.select(FriendsStateSelectors.selectFriendsStateFriends);
  }
  ngOnInit(): void {
    this.store.dispatch(FriendsStateActions.loadFriends());
  }

  navigateToAddFriend(): void {
    this.router.navigate(['/friends/add']);
  }

  joinGame(friend: FriendDto): void {
    // TODO: Implement game invitation logic
    this.router.navigate(['/', 'game', friend.OpenGameId]);
  }

  removeFriend(friend: FriendDto): void {
    if (confirm(`Are you sure you want to remove ${friend.UserName} from your friends?`)) {
      this.store.dispatch(FriendsStateActions.removeFriend({ friendId: friend.UserId }));
    }
  }
}
