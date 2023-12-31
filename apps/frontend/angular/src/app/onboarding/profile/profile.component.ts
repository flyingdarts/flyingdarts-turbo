import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthenticatorService } from '@aws-amplify/ui-angular';
import { AmplifyAuthService } from './../../services/amplify-auth.service';
import { CarouselModel } from './../../shared/carousel/carousel.component';
import { UserProfileStateService } from 'src/app/services/user-profile-state.service';
import { AppStore } from 'src/app/app.store';
import { isNullOrUndefined } from 'src/app/app.component';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit {
  public loading$: Observable<boolean> = this.appStore.select(x=>x.loading);

  public profileForm: FormGroup;
  carouselItems: CarouselModel[] = [
    {
      src: '/assets/registration/social_media.svg',
      title: 'Create an account',
      description: 'You can choose one of the available sign-in methods to create your account.'
    },
    {
      src: '/assets/registration/personal_data.svg',
      title: 'Your profile',
      description: 'Enter a nickname. Enter an email address which will be kept private to FlyingDarts.'
    },
    {
      src: '/assets/registration/video_call.svg',
      title: 'Camera permission',
      description: 'We need permission to access your camera so your dartboard is visible.'
    }
  ];
  constructor(
    private router: Router,
    public authenticator: AuthenticatorService,
    private userProfileStateService: UserProfileStateService,
    private appStore: AppStore,
    private authService: AmplifyAuthService,) {

    this.profileForm = new FormGroup({
      userName: new FormControl('', Validators.required),
      country: new FormControl('', Validators.required),
      email: new FormControl('', [Validators.required, Validators.email])
    })
  }
  ngOnInit(): void {
    if (!isNullOrUndefined(this.userProfileStateService.currentUserProfileDetails)) {
      this.router.navigate(['/', 'lobby'])
    } else {
      this.appStore.setLoading(false);
    }
  }
  async submitForm() {
    if (this.profileForm.valid) {
      var userId = await this.authService.getCognitoUserId();
      var userName = await this.authService.getCognitoName();
      this.appStore.patchProfileState({
        UserName: this.profileForm.value.userName,
        Email: this.profileForm.value.email,
        Country: this.profileForm.value.country,
        CognitoUserId: await this.authService.getCognitoUserId(),
        CognitoUserName: await this.authService.getCognitoName(),
      });
      this.router.navigate(['/', 'onboarding', { outlets: { 'onboarding-outlet': ['camera'] } }])
    }
  }
}
