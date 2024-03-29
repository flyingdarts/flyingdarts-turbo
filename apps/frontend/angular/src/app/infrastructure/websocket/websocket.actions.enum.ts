
export enum WebSocketActions {
  Connect = "connect$",
  Disconnect = "disconnect$",
  Default = "default$",
  UserProfileCreate = "user/profile/create",
  UserProfileUpdate = "user/profile/update",
  UserProfileGet = "user/profile/get",
  X01Create = "games/x01/create",
  X01Queue = "games/x01/queue",
  X01JoinQueue = "games/x01/joinqueue",
  X01Score = "games/x01/score",
  X01Join = "games/x01/join",
  X01WebRTC = "games/x01/webrtc",
  X01WebRTCCandidate = "games/x01/webrtc/candidate"
}
