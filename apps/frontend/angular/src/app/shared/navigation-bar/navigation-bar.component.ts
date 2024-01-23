import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { UserProfileStateService } from './../../services/user-profile-state.service';
import { AnimationOptions } from 'ngx-lottie';
import { isNullOrUndefined } from 'src/app/app.component';
import { Observable } from 'rxjs';
import { AppStore } from 'src/app/app.store';
import { UserProfileApiService } from 'src/app/services/user-profile-api.service';
import { LoginClient } from '@authress/login';
import { UserProfileDetails } from '../models/user-profile-details.model';
import { profile } from 'console';

@Component({
  selector: 'app-navigation-bar',
  templateUrl: './navigation-bar.component.html',
  styleUrls: ['./navigation-bar.component.scss']
})
export class NavigationBarComponent implements OnInit {
  public currentYear: number = new Date().getFullYear();
  public lottieOptions: AnimationOptions = {
    path: '/assets/animations/flyingdarts_icon.json',
    loop: false
  };
  public userName: string = ''; // Initial value is an empty string
  public isAuthenticated!: boolean; // Initial value is false
  public isRegistered!: boolean; // Initial value is false
  private loginClient: LoginClient = new LoginClient({authressLoginHostUrl: "https://authress.flyingdarts.net/", applicationId: "app_2YKyhM6M31XVtuCeuDsSJ2"});

  constructor(
    public router: Router,
    public userProfileService: UserProfileStateService,
    public userApiService: UserProfileApiService,
    private appStore: AppStore,
  ) {

  }

  async ngOnInit() {
    this.appStore.profile$.subscribe((profileDetails) => {
      if (profileDetails !== null) {
        console.log(profileDetails);
        this.isRegistered = true;
        this.isAuthenticated = true;
        this.userName = profileDetails.UserName
      }
    });
  }

  title = 'flyingdarts';

  public async signOut() {
    this.userProfileService.clear();
    await this.loginClient.logout();
  }
}
