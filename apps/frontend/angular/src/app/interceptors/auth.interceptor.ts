import { Injectable } from '@angular/core';
import {
  HttpInterceptor,
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpErrorResponse
} from '@angular/common/http';
import { Observable, throwError, from } from 'rxjs';
import { catchError, switchMap } from 'rxjs/operators';
import { AuthressService } from '../services/authress_service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  constructor(
    private authressService: AuthressService
  ) {}

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return from(this.authressService.getToken()).pipe(
      switchMap((token) => {
        if (token !== null && token !== undefined) {
          const authReq = request.clone({
            setHeaders: {
              Authorization: token
            }
          });
          return next.handle(authReq);
        } else {
          // Handle the case where the token is null or undefined
          // You might want to redirect to the login page or take appropriate action
          return throwError('Token is null or undefined');
        }
      }),
      catchError((error: any) => {
        if (error instanceof HttpErrorResponse && error.status === 401) {
          // Handle 401 Unauthorized error (e.g., redirect to login page)
        }
        return throwError(error);
      })
    );
  }
}
