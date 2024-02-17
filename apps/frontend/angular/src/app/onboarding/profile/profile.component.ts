import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CarouselModel } from './../../shared/carousel/carousel.component';
import { UserProfileStateService } from 'src/app/services/user-profile-state.service';
import { AppStore } from 'src/app/app.store';
import { isNullOrUndefined } from 'src/app/app.component';
import { Observable } from 'rxjs';
import { LoginClient } from '@authress/login';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit {
  public loading$: Observable<boolean>;

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
    private userProfileStateService: UserProfileStateService,
    private appStore: AppStore,
    private activatedRoute: ActivatedRoute) {
      this.loading$ = this.appStore.select(x=>x.loading);
    this.profileForm = new FormGroup({
      userName: new FormControl('', Validators.required),
      country: new FormControl('', Validators.required),
      email: new FormControl('', [Validators.required, Validators.email])
    })
  }
  ngOnInit(): void {
    this.appStore.setLoading(false);
  }

  private loginClient: LoginClient = new LoginClient({authressLoginHostUrl: "https://authress.flyingdarts.net/", applicationId: "app_2YKyhM6M31XVtuCeuDsSJ2"});

  async submitForm() {
    if (this.profileForm.valid) {
      var userIdentity = await this.loginClient.getUserIdentity();
      var userId: string = userIdentity["userId"] as string;
      this.appStore.patchProfileState({
        UserName: this.profileForm.value.userName,
        Email: this.profileForm.value.email,
        Country: this.profileForm.value.country,
        AuthProviderUserId: userId,
      });
      this.router.navigate(['/', 'onboarding', { outlets: { 'onboarding-outlet': ['camera'] } }])
    }
  }
}
