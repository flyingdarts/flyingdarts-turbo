import { Injectable } from '@angular/core';
import { CreateUserProfileCommand } from './../requests/CreateUserProfileCommand';
import { UpdateUserProfileCommand } from './../requests/UpdateUserProfileCommand';
import { HttpClient } from '@angular/common/http';
import { UserProfileDetails } from '../shared/models/user-profile-details.model';
import { Observable, catchError, of } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({ providedIn: 'root' })
export class UserProfileApiService {
  private baseHref = "";
  constructor(private httpClient: HttpClient) {
    this.baseHref = environment.usersApi;
  }
  public createUserProfile(authProviderUserId: string, email: string, userName: string, country: string): Observable<UserProfileDetails> {
    var command: CreateUserProfileCommand = {
      AuthProviderUserId: authProviderUserId,
      UserName: userName,
      Email: email,
      Country: country
    };

    return this.httpClient.post<UserProfileDetails>(`${this.baseHref}/users/profile`, command);
  }

  public getUserProfile(): Observable<UserProfileDetails | null> {
    return this.httpClient.get<UserProfileDetails>(`${this.baseHref}/users/profile`).pipe(
      catchError((error) => {
        if (error.status === 404) {
          // Return null or an Observable of null when the error is a 404
          return of(null);
        }
        // Re-throw the error for any other error types
        throw error;
      })
    );
  }

  public updateUserProfile(email: string, userName: string, country: string): Observable<UserProfileDetails> {
    var command: UpdateUserProfileCommand = {
      UserName: userName,
      Email: email,
      Country: country
    };
  
    return this.httpClient.put<UserProfileDetails>(`${this.baseHref}/users/profile`, command);
  }
}



