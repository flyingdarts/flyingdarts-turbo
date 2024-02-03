import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AuthressService } from '../services/authress_service';
import { AppStore } from '../app.store';

@Injectable({
  providedIn: 'root'
})
export class AuthorizationGuard implements CanActivate {
  constructor(
    private router: Router, 
    private appStore: AppStore,
    private authressService: AuthressService) { }
  async canActivate(): Promise<boolean> {
    
    try {
      const isUserLoggedIn = await this.authressService.isUserLoggedIn();
      console.log('user logged in: ', isUserLoggedIn)
      if (!isUserLoggedIn) {
        this.router.navigate(['/', 'login'])
        return false;
      }
      this.appStore.setLoading(false);
      return true;
    } catch (error) {
      this.router.navigate(['/', 'login'])
      return false;
    }
  }
}
