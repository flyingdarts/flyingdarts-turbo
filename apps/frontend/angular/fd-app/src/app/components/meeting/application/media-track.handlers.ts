import { Injectable } from '@angular/core';
import { TrackHandlers } from '../infrastructure/track.handlers';

@Injectable({ providedIn: 'root' })
export class MediaTrackHandlers implements TrackHandlers {
  localVideoElement!: HTMLVideoElement;
  remoteAudioElement!: HTMLAudioElement;
  remoteVideoElement!: HTMLVideoElement;

  public setupElements(localVideoElement: HTMLVideoElement, remoteAudioElement: HTMLAudioElement, remoteVideoElement: HTMLVideoElement) {
    this.localVideoElement = localVideoElement;
    this.remoteAudioElement = remoteAudioElement;
    this.remoteVideoElement = remoteVideoElement;
  }

  attachLocalVideoTrack(event: { videoEnabled: boolean; videoTrack: MediaStreamTrack }): void {
    if (!event.videoTrack) {
      console.error('No local video track!');
      return;
    }
    if (!this.localVideoElement) {
      console.error('No local video element');
      return;
    }

    this.attachTrack(event.videoTrack, this.localVideoElement as HTMLVideoElement);
  }
  attachRemoteAudioTrack(event: { audioEnabled: boolean; audioTrack: MediaStreamTrack }): void {
    if (!event.audioTrack) {
      console.error('No remote audio track!');
      return;
    }
    if (!this.remoteAudioElement) {
      console.error('No remote audio element');
      return;
    }

    this.attachTrack(event.audioTrack, this.remoteAudioElement);
  }
  attachRemoteVideoTrack(event: { videoEnabled: boolean; videoTrack: MediaStreamTrack }): void {
    if (!event.videoTrack) {
      console.error('No remote video track!');
      return;
    }

    if (!this.remoteVideoElement) {
      console.error('No remote video element');
      return;
    }

    this.attachTrack(event.videoTrack, this.remoteVideoElement);
  }

  private attachTrack(track: MediaStreamTrack, element: HTMLVideoElement | HTMLAudioElement) {
    try {
      const stream = new MediaStream();
      stream.addTrack(track);
      this.setMediaStream(element, stream);
    } catch (error) {
      console.error(error);
    }
  }

  private setMediaStream(element: any, stream: MediaStream) {
    element.srcObject = stream;
    element.onloadedmetadata = () => {
      element.play().catch((error: unknown) => console.error('Error playing media', error));
    };
  }
}
