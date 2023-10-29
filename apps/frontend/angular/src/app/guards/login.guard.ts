import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AmplifyAuthService } from '../services/amplify-auth.service';
import { UserProfileStateService } from '../services/user-profile-state.service';
import { AppStore } from '../app.store';

@Injectable({
  providedIn: 'root'
})
export class LoginGuard implements CanActivate {
  constructor(private store: AppStore) {

  }
  async canActivate(): Promise<boolean> {
    return true;
  }
  
}
