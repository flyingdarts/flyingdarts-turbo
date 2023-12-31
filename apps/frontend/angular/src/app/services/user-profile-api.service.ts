import { Injectable } from '@angular/core';
import { CreateUserProfileCommand } from './../requests/CreateUserProfileCommand';
import { GetUserProfileQuery } from './../requests/GetUserProfileCommand';
import { UpdateUserProfileCommand } from './../requests/UpdateUserProfileCommand';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { UserProfileDetails } from '../shared/models/user-profile-details.model';
import { Observable, catchError, throwError } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Router } from '@angular/router';
@Injectable({ providedIn: 'root' })
export class UserProfileApiService {
  private baseHref = "";
  constructor(private httpClient: HttpClient, private router: Router) {
    this.baseHref = environment.usersApi;
  }
  public createUserProfile(cognitoUserId: string, cognitoUserName: string, email: string, userName: string, country: string): Observable<UserProfileDetails> {
    var command: CreateUserProfileCommand = {
      CognitoUserId: cognitoUserId,
      CognitoUserName: cognitoUserName,
      UserName: userName,
      Email: email,
      Country: country
    };
    return this.httpClient.post<UserProfileDetails>(`${this.baseHref}/users/profile`, command);
  }

  public getUserProfile(cognitoUserName: string): Observable<UserProfileDetails> {
    var query: GetUserProfileQuery = {
      CognitoUserName: cognitoUserName
    };

    return this.httpClient.get<UserProfileDetails>(`${this.baseHref}/users/profile?cognitoUserName=${query.CognitoUserName}`).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error.status === 401) {
          console.error('Unauthorized request. Redirecting to login page...');
          this.router.navigate(['/', 'login']);
        }
        return throwError(() => error); // Re-throw the error to propagate it further
      })
    );
  }

  public updateUserProfile(userId: string, email: string, userName: string, country: string): Observable<UserProfileDetails> {
    var command: UpdateUserProfileCommand = {
      UserId: userId,
      UserName: userName,
      Email: email,
      Country: country
    };
    return this.httpClient.put<UserProfileDetails>(`${this.baseHref}/users/profile`, command);
  }
}



