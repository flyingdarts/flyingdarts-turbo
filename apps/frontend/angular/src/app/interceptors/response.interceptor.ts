import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpResponse, HttpErrorResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';

@Injectable()
export class ResponseInterceptor implements HttpInterceptor {
  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(request).pipe(
      tap({
        next: (event: HttpEvent<any>) => {
          if (event instanceof HttpResponse) {
            // Do something with the successful response
            console.log('ResponseInterceptor - Successful Response:', event);
          }
        },
        error: (error: HttpErrorResponse) => {
          // Do something with the error response
          console.error('ResponseInterceptor - Error Response:', error);
        }
      })
    );
  }
}