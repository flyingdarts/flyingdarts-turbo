import { AfterContentInit, AfterViewInit, Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { isNullOrUndefined } from 'src/app/app.component';
import { AppStore } from 'src/app/app.store';
import { AmplifyAuthService } from 'src/app/services/amplify-auth.service';
import { ApiClient } from 'src/app/services/api.client';
import { UserProfileApiService } from 'src/app/services/user-profile-api.service';
import { UserProfileStateService } from 'src/app/services/user-profile-state.service';
import { UserProfileDetails } from 'src/app/shared/models/user-profile-details.model';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent implements OnInit {
  public loading$: Observable<boolean> = this.store.select(x=>x.loading);
  public loadingTitle: string = 'Welcome back!';
  public loadingSubtitle: string = 'Redirecting you to the lobby.'
  public canLogin: boolean = false;
  public accepted: boolean = false;

  constructor(
    private stateService: UserProfileStateService, 
    private apiService: UserProfileApiService,
    private authService: AmplifyAuthService,
    private router: Router,
    private store: AppStore,
    private apiClient: ApiClient,) {

  }
  accept() {
    this.accepted = !this.accepted;
    this.canLogin = this.accepted;
  }

  login() {
    this.authService.signIn();
  }
  async ngOnInit() {
    console.log("public login oninit")
    this.store.setLoading(true);
    if (this.stateService.currentUserProfileDetails) {
      this.store.setLoading(false);
      await this.router.navigate(['lobby'])
    }
    try {
      var cognitoId = await this.authService.getCognitoId();
      var cognitoName = await this.authService.getCognitoName();
      if (!isNullOrUndefined(cognitoId) && !isNullOrUndefined(cognitoName)) {
        var params = { "cognitoUserName": cognitoName }; // Use the retrieved cognitoUserId in your request parameters
        try {
          var userProfile = await this.apiClient.get<UserProfileDetails>("https://l43qmknl6op6tr7mrwfivpegvm0xvxpr.lambda-url.eu-west-1.on.aws", params);
          userProfile.subscribe((x: UserProfileDetails) => {
            console.log('resolving profile', x);
            if (x == null) {
              console.log('x was null');
            }
          });
        } catch(err) {
          console.log(err);
        }
        console.log('getting profile for', cognitoId, cognitoName);
        this.apiService.getUserProfile(cognitoName);
        this.store.profile$.subscribe(async x=> {
          if (isNullOrUndefined(x)) {
            console.log('profile was null')
            this.store.setProfile({CognitoUserId: cognitoId, CognitoUserName: cognitoName, UserName: '', Email: '', Country: ''})
            await this.router.navigate(['/', 'onboarding', { outlets: { 'onboarding-outlet': ['profile'] } }])
          } else if (x?.UserId != null)  {
            console.log('found profile, routing to lobby')
            this.store.setProfile(x!);
            await this.router.navigate(['lobby'])
          }
        })
      } else {
        console.log('something null or undefined in login oninit')
        this.store.setLoading(false)
      }
    } catch(err){
      console.log('error during oninit login')
      console.log(err)
      this.store.setLoading(false)
    }
  }
}