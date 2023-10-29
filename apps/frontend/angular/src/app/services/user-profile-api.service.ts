import { Injectable } from '@angular/core';

import { WebSocketActions } from '../infrastructure/websocket/websocket.actions.enum';
import { WebSocketMessage } from '../infrastructure/websocket/websocket.message.model';
import { CreateUserProfileCommand } from './../requests/CreateUserProfileCommand';
import { GetUserProfileCommand } from './../requests/GetUserProfileCommand';
import { UpdateUserProfileCommand } from './../requests/UpdateUserProfileCommand';
import { WebSocketMessageService } from '../infrastructure/websocket/websocket-message.service';
@Injectable({ providedIn: 'root' })
export class UserProfileApiService {
  constructor(private webSocketMessagingService: WebSocketMessageService) {

  }
  public createUserProfile(cognitoUserId: string, cognitoUserName: string, email: string, userName: string, country: string): void {
    var message: CreateUserProfileCommand = {
      CognitoUserId: cognitoUserId,
      CognitoUserName: cognitoUserName,
      UserName: userName,
      Email: email,
      Country: country
    };
    let body: WebSocketMessage<CreateUserProfileCommand> = {
      action: WebSocketActions.UserProfileCreate,
      message: message
    };
    this.webSocketMessagingService.sendMessage(JSON.stringify(body));
  }

  public getUserProfile(cognitoUserName: string): void {
    var message: GetUserProfileCommand = {
      CognitoUserName: cognitoUserName
    };
    let body: WebSocketMessage<GetUserProfileCommand> = {
      action: WebSocketActions.UserProfileGet,
      message: message
    };
    this.webSocketMessagingService.sendMessage(JSON.stringify(body));
  }

  public updateUserProfile(userId: string, email: string, userName: string, country: string): void {
    var message: UpdateUserProfileCommand = {
      UserId: userId,
      UserName: userName,
      Email: email,
      Country: country
    };
    let body: WebSocketMessage<UpdateUserProfileCommand> = {
      action: WebSocketActions.UserProfileUpdate,
      message: message
    };
    this.webSocketMessagingService.sendMessage(JSON.stringify(body));
  }
}



