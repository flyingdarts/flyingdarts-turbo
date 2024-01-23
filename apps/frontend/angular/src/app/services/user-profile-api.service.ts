import { Injectable } from '@angular/core';
import { CreateUserProfileCommand } from './../requests/CreateUserProfileCommand';
import { GetUserProfileQuery } from './../requests/GetUserProfileCommand';
import { UpdateUserProfileCommand } from './../requests/UpdateUserProfileCommand';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { UserProfileDetails } from '../shared/models/user-profile-details.model';
import { Observable, catchError, throwError } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Router } from '@angular/router';
import { UserProfileStateService } from './user-profile-state.service';
@Injectable({ providedIn: 'root' })
export class UserProfileApiService {
  private baseHref = "";
  constructor(private httpClient: HttpClient, private router: Router, private stateService: UserProfileStateService) {
    this.baseHref = environment.usersApi;
  }
  public createUserProfile(authProviderUserId: string, email: string, userName: string, country: string): Observable<UserProfileDetails> {
    var command: CreateUserProfileCommand = {
      AuthProviderUserId: authProviderUserId,
      UserName: userName,
      Email: email,
      Country: country
    };
    var headers = { Authorization: this.stateService.idToken };

    return this.httpClient.post<UserProfileDetails>(`${this.baseHref}/users/profile`, command, { headers: headers});
  }

  public getUserProfile(): Observable<UserProfileDetails> {
    var headers = { Authorization: this.stateService.idToken };

    return this.httpClient.get<UserProfileDetails>(`${this.baseHref}/users/profile`, { headers: headers });
  }

  public updateUserProfile(userId: string, email: string, userName: string, country: string): Observable<UserProfileDetails> {
    var command: UpdateUserProfileCommand = {
      UserId: userId,
      UserName: userName,
      Email: email,
      Country: country
    };
    var headers = { Authorization: this.stateService.idToken };

    return this.httpClient.put<UserProfileDetails>(`${this.baseHref}/users/profile`, command, { headers: headers });
  }
}



