import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { Observable } from 'rxjs';
import { UserProfileStateService } from '../services/user-profile-state.service';

@Injectable()
export class AuthenticationInterceptor implements HttpInterceptor {

  constructor(private stateService: UserProfileStateService) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const userToken = this.stateService.idToken;
    
    // Clone the HTTP request and add the authorization header with the token
    const modifiedReq = req.clone({
      setHeaders: { Authorization: `${userToken}` }
    });
    
    // Pass the modified request to the next handler in the chain
    return next.handle(modifiedReq);
  }
}