import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { AnimationOptions } from 'ngx-lottie';
//import { WebSocketActions } from './../../infrastructure/websocket/websocket.actions.enum';
import { AmplifyAuthService } from './../../services/amplify-auth.service';
import { UserProfileApiService } from './../../services/user-profile-api.service';
import { UserProfileStateService } from './../../services/user-profile-state.service';
import { CarouselModel } from './../../shared/carousel/carousel.component';
import { UserProfileDetails } from './../../shared/models/user-profile-details.model';
import { Router } from '@angular/router';
import { AppStore } from 'src/app/app.store';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit {
  public isLoading: boolean = false;
  profileForm: FormGroup;
  carouselItems: CarouselModel[] = [
    {
      src: '/assets/registration/social_media.svg',
      title: 'Username',
      description: 'You can only change your username once every 30 days.'
    },
    {
      src: '/assets/registration/personal_data.svg',
      title: 'Email',
      description: 'We will send you an email with a verification link.'
    },
    {
      src: '/assets/registration/video_call.svg',
      title: 'Camera',
      description: 'Go to the settings page for camera configuration.'
    }
  ];
  constructor(private formBuilder: FormBuilder,
    private apiService: UserProfileApiService,
    private userProfileService: UserProfileStateService,
    private appStore: AppStore,
    private router: Router) {
    this.profileForm = new FormGroup({
      userName: new FormControl('', Validators.required),
      country: new FormControl('', Validators.required),
      email: new FormControl('', [Validators.required, Validators.email])
    });
  }
  public lottieOptions: AnimationOptions = {
    path: '/assets/animations/flyingdarts_icon.json',
    loop: true
  };

  public loadingTitle: string = "Fetching your profile";
  public loadingSubtitle: string = "One moment please.";

  ngOnInit() {
    var profile = this.userProfileService.currentUserProfileDetails;
    this.profileForm = this.formBuilder.group({
      userName: profile.UserName || '',
      country: profile.Country || '',
      email: profile.Email || '',
    });
  }
  updateProfile() {
    if (this.profileForm.valid) {
      this.loadingTitle = "Updating your profile";
      this.isLoading = true;
      this.apiService.updateUserProfile(
        this.userProfileService.currentUserProfileDetails.UserId!,
        this.profileForm.value.email,
        this.profileForm.value.userName,
        this.profileForm.value.country).subscribe(x=> {
          this.appStore.setProfile(x);
          this.userProfileService.currentUserProfileDetails = x;
          this.isLoading = false;
          this.router.navigate(['/', 'lobby'])
        });
    }
  }
}
