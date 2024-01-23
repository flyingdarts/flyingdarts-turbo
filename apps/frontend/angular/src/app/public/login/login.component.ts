import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { isNullOrUndefined } from 'src/app/app.component';
import { AppStore } from 'src/app/app.store';
import { ApiClient } from 'src/app/services/api.client';
import { UserProfileApiService } from 'src/app/services/user-profile-api.service';
import { UserProfileStateService } from 'src/app/services/user-profile-state.service';
import { LoginClient } from "@authress/login";
import { AuthressService } from 'src/app/services/authress_service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent implements OnInit {

  public loading$: Observable<boolean> = this.store.select(x => x.loading);
  public loadingTitle: string = 'Welcome back!';
  public loadingSubtitle: string = 'Redirecting you to the lobby.'
  public canLogin: boolean = false;
  public accepted: boolean = false;
  constructor(
    private store: AppStore,
    private router: Router,
    private authressService: AuthressService,
    private stateService: UserProfileStateService
    ) {

  }
  accept() {
    this.accepted = !this.accepted;
    this.canLogin = this.accepted;
  }


  async login() {
    await this.authressService.authenticate();
  }

  async ngOnInit() {
    try {
      var token = await this.authressService.getToken();
      var userId = this.authressService.getUserId();
      this.stateService.idToken = token;
      console.log('lol', token, userId)
      if (!isNullOrUndefined(userId)) {
        this.router.navigate(['/', 'lobby']);
      }
    } catch(error) {
      console.log(error);
    }
  }
}