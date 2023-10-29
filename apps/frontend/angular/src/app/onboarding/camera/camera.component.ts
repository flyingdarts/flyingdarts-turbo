import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthenticatorService } from '@aws-amplify/ui-angular';
import { WebcamService } from './../../services/webcam.service';
import { AppStore } from 'src/app/app.store';
import { UserProfileApiService } from 'src/app/services/user-profile-api.service';
import { UserProfileStateService } from 'src/app/services/user-profile-state.service';
import { UserProfileDetails } from 'src/app/shared/models/user-profile-details.model';
import { isNullOrUndefined } from 'src/app/app.component';
import { WebSocketService } from 'src/app/infrastructure/websocket/websocket.service';
import { WebSocketActions } from 'src/app/infrastructure/websocket/websocket.actions.enum';
import { CreateX01ScoreCommand } from 'src/app/requests/CreateX01ScoreCommand';
import { JoinGameCommand } from 'src/app/requests/JoinGameCommand';

@Component({
  selector: 'app-camera',
  templateUrl: './camera.component.html',
  styleUrls: ['./camera.component.scss']
})
export class CameraComponent implements OnInit {
  private userProfileDetails: UserProfileDetails | null = null;
  constructor(
    public authenticator: AuthenticatorService,
    private webcamService: WebcamService,
    private userProfileService: UserProfileApiService,
    private userProfileStateService: UserProfileStateService,
    private webSocketService: WebSocketService,
    private router: Router,
    private appStore: AppStore) {

  }
  ngOnInit(): void {
    this.appStore.profile$.subscribe(x => {
      this.userProfileDetails = x;
    });
    this.accessCamera();
  }

  saveCamera() {
    console.log(this.userProfileDetails);
    if (!isNullOrUndefined(this.userProfileDetails)) {
      this.userProfileService.createUserProfile(this.userProfileDetails!.cognitoUserId!, this.userProfileDetails!.cognitoUserName!, this.userProfileDetails!.Email, this.userProfileDetails!.UserName, this.userProfileDetails!.Country);
      this.userProfileStateService.currentUserProfileDetails = this.userProfileDetails;
    }
  }

  async accessCamera() {
    const videoPlayer = document.querySelector('video') as HTMLVideoElement;
    await this.webcamService.requestCameraPermissions().then(stream => {
      videoPlayer.srcObject = stream;
      this.populateCameraSelectList();
      this.appStore.patchProfileState({ cameraPermissionGranted: true })
    }).catch(error => {
      console.error('Failed to attach stream to video element', error);
      this.appStore.patchProfileState({ cameraPermissionGranted: false })
    });
  }

  populateCameraSelectList() {
    if (!this.webcamService.checkCameraPermission()) {
      return;
    }
    // Get the select element and video element from the DOM
    const selectElement = document.querySelector<HTMLSelectElement>('#cameraFormSelectCamera');
    const videoElement = document.querySelector<HTMLVideoElement>('#videoPlayer');

    // Populate the select element with the available webcams
    this.populateWebcamSelectList(selectElement!).then(devices => {
      // Add an event listener to the select element to switch the webcam when the user selects a new option
      selectElement!.addEventListener('change', () => {
        const selectedDeviceId = selectElement!.value;
        this.switchWebcam(selectedDeviceId, videoElement!);
      });
    });
  }

  // Get the available webcams and populate a select element with their names and IDs
  async populateWebcamSelectList(selectElement: HTMLSelectElement): Promise<MediaDeviceInfo[]> {
    // Get the list of available media devices
    const devices = await navigator.mediaDevices.enumerateDevices();

    // Filter the list to only include video input devices
    const videoDevices = devices.filter(device => device.kind === 'videoinput');

    // Create an option element for each video device and add it to the select element
    videoDevices.forEach(device => {
      const option = document.createElement('option');
      option.value = device.deviceId;
      option.text = device.label || `Camera ${selectElement.options.length + 1}`;
      selectElement.appendChild(option);
    });
    console.log(videoDevices);
    // Return the list of video devices
    return videoDevices;
  }

  // Switch the video stream to the selected webcam
  async switchWebcam(selectedDeviceId: string, videoElement: HTMLVideoElement): Promise<void> {
    // Get the current video stream
    const stream = videoElement.srcObject as MediaStream;

    // Stop the current video stream
    if (stream) {
      stream.getTracks().forEach(track => track.stop());
    }

    // Get the media constraints for the selected device
    const constraints: MediaStreamConstraints = {
      video: {
        deviceId: { exact: selectedDeviceId }
      }
    };

    // Get the new video stream
    const newStream = await navigator.mediaDevices.getUserMedia(constraints);

    // Set the new video stream as the source of the video element
    videoElement.srcObject = newStream;
  }
}
