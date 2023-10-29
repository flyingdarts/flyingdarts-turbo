import { Injectable, OnInit } from "@angular/core";
import { Auth } from "aws-amplify";
import { CognitoUser } from "../infrastructure/cognito/cognito-user.model";
import { Observable, from, map, of } from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class AmplifyAuthService {
    public get isAuthenticated$(): Observable<boolean> {
    return from(Auth.currentAuthenticatedUser()).pipe(
      map(user => Boolean(user))
    );
  }
  public async getCognitoUserId(): Promise<string> {
   var userInfo = await Auth.currentUserInfo() as CognitoUser;
    return userInfo.username
  }

  public async getCognitoId(): Promise<string> {
    var userInfo = await Auth.currentAuthenticatedUser();
    return userInfo["attributes"]["sub"]
  }

  public async getCognitoName(): Promise<string> {
    var userInfo = await Auth.currentAuthenticatedUser();
    return userInfo["username"]
  }
  public signIn(): void {
    Auth.federatedSignIn();
  }
  public signOut(): void {
    localStorage.clear();
    Auth.signOut({ global: true });
  }
}

