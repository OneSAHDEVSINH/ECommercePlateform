import { Injectable } from '@angular/core';
import {  HttpRequest,  HttpHandler,  HttpEvent,  HttpInterceptor,  HttpErrorResponse} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from './auth.service';
import { MessageService } from '../general/message.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  constructor(
    private authService: AuthService,
    private messageService: MessageService
  ) { }

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    // Get the token
    const token = this.authService.getToken();

    // Clone the request and add the token if it exists
    if (token) {
      request = request.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`
        }
      });
    }

    // Pass the modified request to the next handler
    return next.handle(request).pipe(
      catchError((error: HttpErrorResponse) => {
        // Handle the error based on status code
        if (error.status === 401) {
          this.messageService.showMessage({
            type: 'error',
            text: 'Session expired. Please login again.'
          });
          this.authService.logout();
        } else if (error.status === 403) {
          // Get the exact error format to understand what's being returned
          console.log('403 Forbidden error:', error);

          let errorMessage = 'You don\'t have permission to access this resource.';

          // Check for common error response patterns
          if (error.error) {
            if (typeof error.error === 'string') {
              errorMessage = error.error;
            } else if (error.error.message) {
              errorMessage = error.error.message;
            } else if (error.error.title) {
              errorMessage = error.error.title;
            }
          }

          // Show the user a clean error message
          this.messageService.showMessage({
            type: 'error',
            text: errorMessage
          });

          // Redirect to the access-denied page if needed
          // (Uncomment if redirection is prefered)
          // this.router.navigate(['/access-denied']);

          return throwError(() => error);
        } else if (error.status === 0) {
          this.messageService.showMessage({
            type: 'error',
            text: 'Server is unreachable. Please check your connection.'
          });
        } else if (error.status === 409) {
          // This is specifically for DuplicateResourceException (HTTP 409 Conflict)
          const errorMessage = error.error?.message || 'A duplicate item was found.';
          this.messageService.showMessage({
            type: 'error',
            text: errorMessage
          });
        } else {
          // Extract error message from response if available
          const errorMessage = error.error?.message ||
            error.error?.title ||
            error.message ||
            'An unexpected error occurred';

          this.messageService.showMessage({
            type: 'error',
            text: errorMessage
          });
        }

        return throwError(() => error);
      })
    );
  }
}
