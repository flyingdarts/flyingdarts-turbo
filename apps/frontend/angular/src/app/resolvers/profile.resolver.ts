import { Injectable } from '@angular/core';
import {
  Router, Resolve,
  RouterStateSnapshot,
  ActivatedRouteSnapshot
} from '@angular/router';
import { Observable, of } from 'rxjs';
import { WebSocketService } from '../infrastructure/websocket/websocket.service';
import { AppStore } from '../app.store';
import { WebSocketActions } from '../infrastructure/websocket/websocket.actions.enum';
import { UserProfileDetails } from '../shared/models/user-profile-details.model';
import { isNullOrUndefined } from '../app.component';

@Injectable({
  providedIn: 'root'
})
export class ProfileResolver implements Resolve<boolean> {
  constructor(private webSocketService: WebSocketService, private appStore: AppStore, private router: Router) {}
  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> {
    return new Observable<boolean>((observer) => {
      const profileSubscription = this.webSocketService.getMessages().subscribe(x=> {
        
        if (x.action === WebSocketActions.UserProfileGet && isNullOrUndefined(x.message as UserProfileDetails)) {
            observer.next(true);
            observer.complete();
            if (route.outlet == 'onboarding-outlet') {
              this.appStore.setLoading(false);
            }
        } else {
          this.router.navigate(['/', 'lobby'])
          observer.next(false);
          observer.complete();
        }
      });
      return () => {
        profileSubscription.unsubscribe();
      };
    })
  }
}
