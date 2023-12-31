import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { Observable } from 'rxjs';
import { UserProfileStateService } from '../services/user-profile-state.service';
@Injectable({
  providedIn: 'root'
})
export class UserTokenInterceptor implements HttpInterceptor {
  /**
   *
   */
  constructor(private stateService: UserProfileStateService) {}
  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const token = this.stateService.idToken;
    if (token && token.length >  0) {
      req = req.clone({
        setHeaders: {
          Authorization: token
        }
      });
    }
    return next.handle(req);
  }
}
