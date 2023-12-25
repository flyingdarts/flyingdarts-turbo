import { Injectable } from '@angular/core';

import { WebSocketActions } from '../infrastructure/websocket/websocket.actions.enum';
import { WebSocketMessage } from '../infrastructure/websocket/websocket.message.model';
import { CreateUserProfileCommand } from './../requests/CreateUserProfileCommand';
import { GetUserProfileQuery } from './../requests/GetUserProfileCommand';
import { UpdateUserProfileCommand } from './../requests/UpdateUserProfileCommand';
import { WebSocketMessageService } from '../infrastructure/websocket/websocket-message.service';
import { HttpClient } from '@angular/common/http';
import { UserProfileDetails } from '../shared/models/user-profile-details.model';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
@Injectable({ providedIn: 'root' })
export class UserProfileApiService {
  private baseHref = "";
  constructor(private httpClient: HttpClient) {
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
    return this.httpClient.get<UserProfileDetails>(`${this.baseHref}/users/profile?cognitoUserName=${query.CognitoUserName}`);
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



