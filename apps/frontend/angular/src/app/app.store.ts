import { ComponentStore, OnStateInit, OnStoreInit } from "@ngrx/component-store";
import { UserProfileDetails } from "./shared/models/user-profile-details.model";
import { Observable, of } from "rxjs";
import { WebSocketService } from "./infrastructure/websocket/websocket.service";
import { WebSocketActions } from "./infrastructure/websocket/websocket.actions.enum";
import { Injectable } from "@angular/core";
import { AppState, initialApplicationState } from "./app.state";
import { UserProfileStateService } from "./services/user-profile-state.service";
import { Router } from "@angular/router";
import { PreferedX01SettingsService } from "./services/prefered-x01-settings.service";

@Injectable()
export class AppStore extends ComponentStore<AppState> implements OnStoreInit {
  constructor(private webSocketService: WebSocketService, private userProfileService: UserProfileStateService, private router: Router,private settingsService: PreferedX01SettingsService) {
    var initState = initialApplicationState;
    var preferedSets = settingsService.preferedX01Sets;
    var preferedLegs = settingsService.preferedX01Legs;
    if (!!preferedSets && !!preferedLegs) {
      initState.preferedSettings.x01Sets = preferedSets;
      initState.preferedSettings.x01Legs = preferedLegs;
    }
    super(initState);
  }

  ngrxOnStoreInit() {
    this.webSocketService.getMessages().subscribe(x => {
      switch (x.action) {
        case WebSocketActions.UserProfileGet:
        case WebSocketActions.UserProfileCreate:
        case WebSocketActions.UserProfileUpdate:
          if (x.message != null) {
            console.log('got user profile message in ngrxOnStoreInit', x.message);
            this.setProfile(x.message as UserProfileDetails);
            this.userProfileService.currentUserProfileDetails = x.message as UserProfileDetails;
            this.setLoading(false);
            if (x.action === WebSocketActions.UserProfileCreate) {
              console.log('got create, navigating to lobby...')
              this.router.navigate(['lobby'])
            }
          }
      }
    });
  }

  readonly loading$: Observable<boolean> = this.select(x => x.loading);
  readonly profile$: Observable<UserProfileDetails | null> = this.select(x => x.profile);
  readonly preferedX01Sets$: Observable<number> = this.select(x=>x.preferedSettings.x01Sets);
  readonly preferedX01Legs$: Observable<number> = this.select(x=>x.preferedSettings.x01Legs);
  readonly setLoading = this.updater((state, value: boolean) => ({...state, loading: value}));
  readonly setProfile = this.updater((state, value: UserProfileDetails) => ({ ...state, profile: value }));

  readonly increasePreferedX01Sets = this.updater((state) => ({...state, preferedSettings: {...state.preferedSettings, x01Sets: state.preferedSettings.x01Sets + 2}}))
  readonly decreasePreferedX01Sets = this.updater((state) => ({...state, preferedSettings: {...state.preferedSettings, x01Sets: state.preferedSettings.x01Sets - 2}}))
  readonly increasePreferedX01Legs = this.updater((state) => ({...state, preferedSettings: {...state.preferedSettings, x01Legs: state.preferedSettings.x01Legs + 2}}))
  readonly decreasePreferedX01Legs = this.updater((state) => ({...state, preferedSettings: {...state.preferedSettings, x01Legs: state.preferedSettings.x01Legs - 2}}))
  readonly setPreferedX01Sets = this.updater((state, value: number) => ({...state, preferedSettings: {  ...state.preferedSettings, x01Sets: value}}))
  readonly setPreferedX01Legs = this.updater((state, value: number) => ({...state, preferedSettings: {  ...state.preferedSettings, x01Legs: value}}))

  patchProfileState(newProfileValues: any) {
    this.patchState((state) => ({
      profile: { ...state.profile, ...newProfileValues }
    }));
  }
}
