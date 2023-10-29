import { Injectable, OnDestroy } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class WebcamService implements OnDestroy {
  private cameraStream: MediaStream | null = null;
  constructor() { 

  }
  ngOnDestroy(): void {
    this.disposeWebcamStream()
  }

  async checkCameraPermission(): Promise<boolean> {
    if (!navigator.permissions || !navigator.permissions.query) {
      // Permissions API not supported, assume permission was granted
      return true;
    }
    const permissionStatus = await navigator.permissions.query({ name: 'camera' as PermissionName });
    return permissionStatus.state === 'granted';
  }

  async requestCameraPermissions(): Promise<MediaStream> {
    try {
      const stream = await navigator.mediaDevices.getUserMedia({ video: true });
      this.cameraStream = stream;
      return stream;
    } catch (error) {
      console.error('Camera access denied', error);
      throw error;
    }
  }

  disposeWebcamStream(): void {
    if (this.cameraStream) {
      const tracks = this.cameraStream.getTracks();
      tracks.forEach(track => track.stop()); // Stop all tracks in the stream
      this.cameraStream = null; // Clear the stored stream
      console.log('Camera stream disposed');
    }
  }
}
