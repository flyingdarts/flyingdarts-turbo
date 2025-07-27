import { Injectable } from "@angular/core";
import { Actions, createEffect, ofType } from "@ngrx/effects";
import { FriendsService } from "../../services/friends.service";
import {
  FriendsActions,
  loadFriendRequestsFailure,
  loadFriendRequestsSuccess,
  loadFriendsFailure,
  loadFriendsSuccess,
  removeFriendFailure,
  removeFriendSuccess,
  sendFriendRequestFailure,
  sendFriendRequestSuccess,
} from "./friends.actions";
import { catchError, map, switchMap } from "rxjs/operators";
import { of } from "rxjs";

@Injectable()
export class FriendsEffects {
  constructor(
    private actions$: Actions,
    private friendsService: FriendsService
  ) {}

  loadFriends$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(FriendsActions.LoadFriends),
      switchMap(() =>
        this.friendsService.loadFriends().pipe(
          map((friends) => loadFriendsSuccess({ friends })),
          catchError((error) => of(loadFriendsFailure({ error })))
        )
      )
    );
  });

  loadFriendRequests$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(FriendsActions.LoadFriendRequests),
      switchMap(() =>
        this.friendsService.loadFriendRequests().pipe(
          map((friendRequests) =>
            loadFriendRequestsSuccess({ friendRequests })
          ),
          catchError((error) => of(loadFriendRequestsFailure({ error })))
        )
      )
    );
  });

  removeFriend$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(FriendsActions.RemoveFriend),
      switchMap(({ friendId }) =>
        this.friendsService.removeFriend(friendId).pipe(
          map(() => removeFriendSuccess({ friendId })),
          catchError((error) => of(removeFriendFailure({ error })))
        )
      )
    );
  });

  sendFriendRequest$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(FriendsActions.SendFriendRequest),
      switchMap(({ friendId }) =>
        this.friendsService.sendFriendRequest(friendId).pipe(
          map(() => sendFriendRequestSuccess({ friendId })),
          catchError((error) => of(sendFriendRequestFailure({ error })))
        )
      )
    );
  });
}
