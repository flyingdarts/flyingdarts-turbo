import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Resolve, RouterStateSnapshot } from '@angular/router';
import { UserProfileApiService } from '../services/user-profile-api.service';
import { UserProfileDetails } from '../shared/models/user-profile-details.model';
import { Observable } from 'rxjs';
import { AppStore } from '../app.store';
import { finalize, tap } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class ProfileDetailsResolver implements Resolve<UserProfileDetails> {
  constructor(private userApiService: UserProfileApiService, private appStore: AppStore) {}

  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<UserProfileDetails> {
    this.appStore.setLoading(true);
    return this.userApiService.getUserProfile().pipe(
      tap(profile => this.appStore.setProfile(profile)),
      finalize(() => this.appStore.setLoading(false))
    );
  }
}