import { Injectable } from "@angular/core";
import { createEffect, ofType, Actions } from "@ngrx/effects";
import { Hub } from "aws-amplify";
import { tap } from "rxjs";
import * as AuthActions from './auth.actions';
import { UserProfileApiService } from "./services/user-profile-api.service";
import { AmplifyAuthService } from "./services/amplify-auth.service";
import { isNullOrUndefined } from "./app.component";

@Injectable()
export class AuthEffects {
  listenForAuthEvents$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.listenForAuthEvents),
      tap(() => {
        Hub.listen('auth', async ({ payload: { event } }) => {
          switch (event) {
            case 'signIn':
              console.log('ti signin hallo', event);
              var cognitoName = await this.authService.getCognitoName();
              if (!isNullOrUndefined(cognitoName)) {
                this.apiService.getUserProfile(cognitoName);
              }
              break;
            case 'signOut':
              break;
            default:
              break;
          }
        });
      })
    ),
    { dispatch: false } // Set dispatch to false to prevent re-dispatching actions
  );

  constructor(private actions$: Actions, private authService: AmplifyAuthService, private apiService: UserProfileApiService) {}
}
