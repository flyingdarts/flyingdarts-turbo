import { Injectable } from '@angular/core';
import {
  ActivatedRouteSnapshot,
  CanActivate,
  Router,
  RouterStateSnapshot,
} from '@angular/router';

@Injectable({
  providedIn: 'root',
})
export class X01GameSettingsGuard implements CanActivate {
  constructor(private readonly router: Router) {}

  async canActivate(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Promise<boolean> {
    if (!localStorage.getItem('X01GameSettings')) {
      await this.router.navigate(['', 'settings']);
      return false;
    }

    return true;
  }
}
