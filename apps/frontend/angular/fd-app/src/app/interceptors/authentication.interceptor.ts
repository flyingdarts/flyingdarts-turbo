import { HttpHandlerFn, HttpRequest } from '@angular/common/http';
import { inject } from '@angular/core';
import { Observable, from } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { AuthressService } from '../services/authress.service';

export function authInterceptor(
  req: HttpRequest<unknown>,
  next: HttpHandlerFn
): Observable<any> {
  // Inject the current `AuthressService` and use it to get an authentication token
  const authressService = inject(AuthressService);

  // Convert the promise to an observable
  return from(authressService.getToken()).pipe(
    switchMap((userToken) => {
      // Clone the HTTP request and add the authorization header with the token
      const modifiedReq = req.clone({
        setHeaders: { Authorization: `${userToken}` },
      });

      // Pass the modified request to the next handler in the chain
      return next(modifiedReq);
    })
  );
}
