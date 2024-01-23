import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot } from '@angular/router';
import { ApiClient } from '../services/api.client';

@Injectable({
  providedIn: 'root'
})
export class OnboardingGuard implements CanActivate {
  constructor(
    private router: Router,
    private apiClient: ApiClient,
  ) {

  }
  async canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Promise<boolean> {

    return true;
  }
  // async canActivate(
  //   route: ActivatedRouteSnapshot,
  //   state: RouterStateSnapshot) {


  //   if (!this.stateService.currentUserProfileDetails.isRegistered!) {
  //     return true;
  //   } else {
  //     if (!this.stateService.currentUserProfileDetails.cameraPermissionGranted!) {
  //       this.router.navigate(['/', 'onboarding', { outlets: { 'onboarding-outlet': ['camera'] } }]);
  //       return true;
  //     } else {
  //       this.router.navigate(['/', 'lobby']);
  //       return false;
  //     }
  //   }
  // }

}
