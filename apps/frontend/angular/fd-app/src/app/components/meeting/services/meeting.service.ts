import { Injectable } from '@angular/core';
import DyteClient, { DyteParticipant } from '@dytesdk/web-core';
import { MediaTrackHandlers } from '../application/media-track.handlers';
import { DyteClientFactory } from '../factories/dyte-client.factory';

@Injectable({ providedIn: 'root' })
export class MeetingService {
  dyteClient!: DyteClient;

  constructor(private readonly dyteClientFactory: DyteClientFactory, private readonly trackHandlers: MediaTrackHandlers) {}

  public setupElements(localVideoElement: HTMLVideoElement, remoteAudioElement: HTMLAudioElement, remoteVideoElement: HTMLVideoElement) {
    this.trackHandlers.setupElements(localVideoElement, remoteAudioElement, remoteVideoElement);
  }

  async create(token: string): Promise<DyteClient> {
    const client = await this.dyteClientFactory.create(token);

    client.self.on('videoUpdate', event => {
      return this.trackHandlers.attachLocalVideoTrack(event);
    });
    client.participants.joined.on('audioUpdate', (participant: DyteParticipant) => {
      return this.trackHandlers.attachRemoteAudioTrack(participant);
    });
    client.participants.joined.on('videoUpdate', participant => {
      return this.trackHandlers.attachRemoteVideoTrack(participant);
    });

    this.trackHandlers.attachLocalVideoTrack({
      videoEnabled: true,
      videoTrack: client.self.videoTrack,
    });

    return client;
  }
}
