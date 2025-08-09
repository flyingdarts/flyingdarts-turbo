import { createReducer, on } from '@ngrx/store';
import {
  loadFriendRequests,
  loadFriendRequestsFailure,
  loadFriendRequestsSuccess,
  loadFriends,
  loadFriendsFailure,
  loadFriendsSuccess,
  removeFriend,
  removeFriendFailure,
  removeFriendSuccess,
  sendFriendRequest,
  sendFriendRequestFailure,
  sendFriendRequestSuccess,
} from './friends.actions';
import { initialFriendsState } from './friends.state';

export const friendsReducer = createReducer(
  initialFriendsState,
  on(loadFriends, state => ({
    ...state,
    isLoading: true,
  })),
  on(loadFriendsSuccess, (state, { friends }) => ({
    ...state,
    friends,
    isLoading: false,
  })),
  on(loadFriendsFailure, (state, { error }) => ({
    ...state,
    error,
    isLoading: false,
  })),
  on(loadFriendRequests, state => ({
    ...state,
    isLoading: true,
  })),
  on(loadFriendRequestsSuccess, (state, { friendRequests }) => ({
    ...state,
    friendRequests: friendRequests,
    isLoading: false,
  })),
  on(loadFriendRequestsFailure, (state, { error }) => ({
    ...state,
    error,
    isLoading: false,
  })),
  on(removeFriend, state => ({
    ...state,
    isLoading: true,
  })),
  on(removeFriendSuccess, (state, { friendId }) => ({
    ...state,
    friends: state.friends.filter(friend => friend.UserId !== friendId),
    isLoading: false,
  })),
  on(removeFriendFailure, (state, { error }) => ({
    ...state,
    error,
    isLoading: false,
  })),
  on(sendFriendRequest, state => ({
    ...state,
    isLoading: true,
  })),
  on(sendFriendRequestSuccess, (state, { friendId }) => ({
    ...state,
    friendRequests: {
      ...state.friendRequests,
      OutgoingRequests: [
        ...state.friendRequests.OutgoingRequests, // ,friendId
      ],
    },
    isLoading: false,
  })),
  on(sendFriendRequestFailure, (state, { error }) => ({
    ...state,
    error,
    isLoading: false,
  }))
);
