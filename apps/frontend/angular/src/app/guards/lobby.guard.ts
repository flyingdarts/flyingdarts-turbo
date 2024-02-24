import { Injectable, inject } from "@angular/core";
import { CanActivateFn, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from "@angular/router";
import { isNullOrUndefined } from "../app.component";
import { UserProfileStateService } from "../services/user-profile-state.service";
import { AppStore } from "../app.store";

@Injectable({providedIn: 'root'})
export class LobbyPermissionsService {
    constructor(private stateService: UserProfileStateService, private router: Router, private appStore: AppStore) { }

    canActivate(): boolean {
        try {
            if (isNullOrUndefined(this.stateService.idToken)) {
                console.log('[LobbyGuard] Not authenticated, navigating to login')
                this.router.navigate(['/', 'login']);
                return false;
            }
            console.log('[LobbyGuard] Authenticated, navigating to lobby')
            return true;
        } catch {
            console.log('[LobbyGuard] Error during authentication, navigating to login')
            this.router.navigate(['/', 'login']);
            return false;
        }
    }
}

export const canActivateLobby: CanActivateFn = (
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot,
) => {
    return inject(LobbyPermissionsService).canActivate();
};