import { createAction, props } from '@ngrx/store';
import { FriendDto, FriendRequestResponseDto } from '../../dtos/friend.dto';

export enum FriendsActions {
  LoadFriends = '[Friends] Load Friends',
  LoadFriendsSuccess = '[Friends] Load Friends Success',
  LoadFriendsFailure = '[Friends] Load Friends Failure',
  LoadFriendRequests = '[Friends] Load Friend Requests',
  LoadFriendRequestsSuccess = '[Friends] Load Friend Requests Success',
  LoadFriendRequestsFailure = '[Friends] Load Friend Requests Failure',
  RemoveFriend = '[Friends] Remove Friend',
  RemoveFriendSuccess = '[Friends] Remove Friend Success',
  RemoveFriendFailure = '[Friends] Remove Friend Failure',
  SendFriendRequest = '[Friends] Send Friend Request',
  SendFriendRequestSuccess = '[Friends] Send Friend Request Success',
  SendFriendRequestFailure = '[Friends] Send Friend Request Failure',
}

export const loadFriends = createAction(FriendsActions.LoadFriends);
export const loadFriendsSuccess = createAction(FriendsActions.LoadFriendsSuccess, props<{ friends: FriendDto[] }>());
export const loadFriendsFailure = createAction(FriendsActions.LoadFriendsFailure, props<{ error: any }>());

export const loadFriendRequests = createAction(FriendsActions.LoadFriendRequests);
export const loadFriendRequestsSuccess = createAction(
  FriendsActions.LoadFriendRequestsSuccess,
  props<{ friendRequests: FriendRequestResponseDto }>()
);
export const loadFriendRequestsFailure = createAction(FriendsActions.LoadFriendRequestsFailure, props<{ error: any }>());

export const removeFriend = createAction(FriendsActions.RemoveFriend, props<{ friendId: string }>());
export const removeFriendSuccess = createAction(FriendsActions.RemoveFriendSuccess, props<{ friendId: string }>());
export const removeFriendFailure = createAction(FriendsActions.RemoveFriendFailure, props<{ error: any }>());

export const sendFriendRequest = createAction(FriendsActions.SendFriendRequest, props<{ friendId: string }>());
export const sendFriendRequestSuccess = createAction(FriendsActions.SendFriendRequestSuccess, props<{ friendId: string }>());
export const sendFriendRequestFailure = createAction(FriendsActions.SendFriendRequestFailure, props<{ error: any }>());
