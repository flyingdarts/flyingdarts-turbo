export interface TrackHandlers {
  attachLocalVideoTrack(event: {
    videoEnabled: boolean;
    videoTrack: MediaStreamTrack;
  }): void;
  attachRemoteAudioTrack(event: {
    audioEnabled: boolean;
    audioTrack: MediaStreamTrack;
  }): void;
  attachRemoteVideoTrack(event: {
    videoEnabled: boolean;
    videoTrack: MediaStreamTrack;
  }): void;
}
