export class FriendDto {
  UserId!: string;
  UserName!: string;
  Email!: string;
  Country!: string;
  FriendsSince!: string;
  IsOnline!: boolean;
  ConnectionId?: string;
  Picture?: string;
  OpenGameId?: string;
}

export class FriendRequestDto {
  RequestId!: string;
  RequesterId!: string;
  RequesterUserName!: string;
  TargetUserId!: string;
  Message?: string;
  CreatedAt!: string;
  Status!: FriendRequestStatus;
}

export interface FriendRequestsDto {
  IncomingRequests: FriendRequestDto[];
  OutgoingRequests: FriendRequestDto[];
}

export class FriendRequestResponseDto implements FriendRequestsDto {
  IncomingRequests!: FriendRequestDto[];
  OutgoingRequests!: FriendRequestDto[];

  get any(): boolean {
    return this.IncomingRequests.length > 0 || this.OutgoingRequests.length > 0;
  }

  get length(): number {
    return this.IncomingRequests.length + this.OutgoingRequests.length;
  }
}

export class UserSearchDto {
  UserId!: string;
  UserName!: string;
  Country!: string;
  IsAlreadyFriend!: boolean;
  HasPendingRequest!: boolean;
  Picture?: string;
}

export enum FriendRequestStatus {
  Pending = 'Pending',
  Accepted = 'Accepted',
  Declined = 'Declined',
  Cancelled = 'Cancelled',
}
