import { Injectable, inject } from "@angular/core";
import { CanActivateFn, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from "@angular/router";
import { isNullOrUndefined } from "../app.component";
import { UserProfileStateService } from "../services/user-profile-state.service";

@Injectable({ providedIn: 'root' })
export class LoginPermissionsService {
    constructor(private stateService: UserProfileStateService, private router: Router) { }

    canActivate(): boolean {
        if (!isNullOrUndefined(this.stateService.idToken)) {
            console.log('[LoginGuard] Token found, navigating to lobby')
            this.router.navigate(['/', 'lobby']);
            return false;
        }
        console.log('[LoginGuard] Token not found, navigating to login')
        return true;

    }
}

export const canActivateLogin: CanActivateFn = (
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot,
) => {
    return inject(LoginPermissionsService).canActivate();
};