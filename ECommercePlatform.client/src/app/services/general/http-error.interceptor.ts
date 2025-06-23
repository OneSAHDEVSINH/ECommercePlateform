import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { MessageService } from './message.service';
import { Router } from '@angular/router';
import { AuthService } from '../auth/auth.service';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const messageService = inject(MessageService);
  const router = inject(Router);
  const authService = inject(AuthService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      console.log('Error intercepted:', error);

      // Handle based on status code
      switch (error.status) {
        case 401:
          messageService.showMessage({
            type: 'error',
            text: 'Your session has expired. Please login again.'
          });
          authService.logout();
          router.navigate(['/admin/login']);
          break;

        case 403:
          console.log('403 Forbidden details:', {
            url: error.url,
            errorObject: error.error,
            headers: error.headers
          });

          let forbiddenMessage = 'Access denied. You don\'t have permission for this action.';

          // Try to extract a more specific message
          if (error.error) {
            if (typeof error.error === 'string') {
              try {
                // Try to parse as JSON if it's a string
                const parsed = JSON.parse(error.error);
                forbiddenMessage = parsed.message || parsed.title || forbiddenMessage;
              } catch {
                // If it's a plain string, use it directly
                forbiddenMessage = error.error;
              }
            }
            else if (error.error.message) {
              forbiddenMessage = error.error.message;
            }
            else if (error.error.title) {
              forbiddenMessage = error.error.title;
            }
          }

          messageService.showMessage({
            type: 'error',
            text: forbiddenMessage
          });

          // Optionally navigate to access-denied page
          // router.navigate(['/access-denied']);
          break;

        case 0:
          messageService.showMessage({
            type: 'error',
            text: 'Cannot connect to server. Please check your connection.'
          });
          break;

        default:
          // For other error codes
          let errorMessage = 'An error occurred';
          if (error.error) {
            if (typeof error.error === 'string') {
              errorMessage = error.error;
            } else if (error.error.message) {
              errorMessage = error.error.message;
            } else if (error.error.title) {
              errorMessage = error.error.title;
            }
          } else if (error.message) {
            errorMessage = error.message;
          }

          messageService.showMessage({
            type: 'error',
            text: errorMessage
          });
      }

      return throwError(() => error);
    })
  );
};
