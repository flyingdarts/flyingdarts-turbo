import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { Auth } from 'aws-amplify';

@Injectable({
  providedIn: 'root'
})
export class AuthorizationGuard implements CanActivate {
  constructor(private router: Router) { }
  async canActivate(): Promise<boolean> {
    try {
      await Auth.currentAuthenticatedUser();
      return true;
    } catch (error) {
      this.router.navigate(['/', 'login'])
      return false;
    }
  }
}
