import { createFeatureSelector, createSelector } from '@ngrx/store';
import { FriendsState } from './friends.state';

// Selectors
export const selectFriendsState = createFeatureSelector<FriendsState>('friendsState');

export const selectFriendsStateLoading = createSelector(selectFriendsState, (state: FriendsState) => state.isLoading);

export const selectFriendsStateFriends = createSelector(selectFriendsState, (state: FriendsState) => state.friends);

export const selectFriendsStateFriendRequests = createSelector(selectFriendsState, (state: FriendsState) => state.friendRequests);
