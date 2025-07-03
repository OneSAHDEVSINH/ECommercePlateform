import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ErrorHandlerService {

  constructor() { }

  handleError(error: any): void {
    // Filter out the ExpressionChangedAfterItHasBeenCheckedError for reference ID
    if (error.message && error.message.includes('Expression has changed after it was checked') &&
      error.message.includes('AccessDeniedComponent')) {
      // Suppress this specific error as it's harmless and we've fixed the root cause
      return;
    }

    console.error('Global error:', error);
  }
}
