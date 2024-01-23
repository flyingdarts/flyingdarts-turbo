import { Injectable } from '@angular/core';
import { ActivatedRoute, CanActivate, Router } from '@angular/router';
import { LoginClient } from '@authress/login';
import { UserProfileStateService } from '../services/user-profile-state.service';
import { AuthressService } from '../services/authress_service';

@Injectable({
  providedIn: 'root'
})
export class AuthorizationGuard implements CanActivate {
  constructor(private router: Router, private route: ActivatedRoute, private stateService: UserProfileStateService, private authressService: AuthressService) { }
  async canActivate(): Promise<boolean> {
    
    try {
      console.log(this.route.snapshot);
      const loginClient: LoginClient = new LoginClient({authressLoginHostUrl: "https://authress.flyingdarts.net/", applicationId: "app_2YKyhM6M31XVtuCeuDsSJ2"});
      const isUserLoggedIn = await loginClient.userSessionExists();
      if (!isUserLoggedIn) {
        console.log('# not logged in')
        this.router.navigate(['/', 'login'])
        return false;
      }
      console.log("# logged in")
      return true;
    } catch (error) {
      console.log('error # not logged in')
      this.router.navigate(['/', 'login'])
      return false;
    }
  }
}
