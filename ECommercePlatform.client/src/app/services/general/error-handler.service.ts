import { ErrorHandler, Injectable, inject } from '@angular/core';
import { Router } from '@angular/router';

@Injectable()
export class GlobalErrorHandler implements ErrorHandler {
  private router = inject(Router);

  handleError(error: Error): void {
    if (error.message && error.message.includes('URI malformed')) {
      console.error('Malformed URI detected, redirecting to login');
      // Navigate to login without the malformed return URL
      this.router.navigate(['/admin/login']);
    } else {
      // Log other errors
      console.error('Global error:', error);
    }
  }
}
