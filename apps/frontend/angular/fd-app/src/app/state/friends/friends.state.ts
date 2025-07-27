import { FriendDto, FriendRequestsDto } from "../../dtos/friend.dto";

export interface FriendsState {
  friends: FriendDto[];
  friendRequests: FriendRequestsDto;
  isLoading: boolean;
}

export const initialFriendsState: FriendsState = {
  friends: [],
  friendRequests: {
    IncomingRequests: [],
    OutgoingRequests: [],
  },
  isLoading: false,
};
