
export enum WebSocketActions {
  Connect = "connect$",
  Disconnect = "disconnect$",
  Default = "default$",
  RoomsOnCreate = "rooms/create",
  UserProfileCreate = "v2/user/profile/create",
  UserProfileUpdate = "v2/user/profile/update",
  UserProfileGet = "v2/user/profile/get",
  X01Create = "v2/games/x01/create",
  X01JoinQueue = "v2/games/x01/joinqueue",
  X01Score = "v2/games/x01/score",
  X01Join = "v2/games/x01/join",
}
