import { Injectable, inject } from "@angular/core";
import { CanActivateFn, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from "@angular/router";
import { isNullOrUndefined } from "../app.component";
import { UserProfileStateService } from "../services/user-profile-state.service";

@Injectable({providedIn: 'root'})
export class LoginPermissionsService {
    constructor(private stateService: UserProfileStateService, private router: Router) { }

    canActivate(): boolean {
        try {
            if (!isNullOrUndefined(this.stateService.idToken)) {
                this.router.navigate(['/', 'lobby']);
                return false;
            }
            return true;
        } catch {
            return true;
        }
    }
}

export const canActivateLogin: CanActivateFn = (
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot,
) => {
    return inject(LoginPermissionsService).canActivate();
};