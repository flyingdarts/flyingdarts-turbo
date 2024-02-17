import { inject } from "@angular/core";
import { ResolveFn, ActivatedRouteSnapshot, RouterStateSnapshot, provideRouter } from "@angular/router";
import { UserProfileDetails } from "../shared/models/user-profile-details.model";
import { UserProfileApiService } from "../services/user-profile-api.service";

export const userProfileResolver: ResolveFn<UserProfileDetails | null> = (
  route: ActivatedRouteSnapshot,
  state: RouterStateSnapshot,
) => {
  return inject(UserProfileApiService).getUserProfile();
};