import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { UserProfileStateService } from '../services/user-profile-state.service';

@Injectable({
  providedIn: 'root'
})
export class OnboardingGuard implements CanActivate {
  constructor(private stateService: UserProfileStateService, private router: Router) {

  }
  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
      if (!this.stateService.currentUserProfileDetails.isRegistered!) {
        return true;
      } else {
        if (!this.stateService.currentUserProfileDetails.cameraPermissionGranted!) {
          this.router.navigate(['/', 'onboarding', { outlets: { 'onboarding-outlet': ['camera']}}]);
          return true;
        } else {
          this.router.navigate(['/', 'lobby']);
          return false;
        }
      }
    }
  
}
