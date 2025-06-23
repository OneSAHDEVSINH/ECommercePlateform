import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

export interface PermissionError {
  module: string;
  permission: string;
  message: string;
  timestamp: Date;
}

@Injectable({
  providedIn: 'root'
})
export class PermissionNotificationService {
  private errorSubject = new BehaviorSubject<PermissionError | null>(null);

  public error$ = this.errorSubject.asObservable();

  /**
   * Show a permission error message
   */
  showPermissionError(module: string, permission: string, customMessage?: string): void {
    const message = customMessage ||
      `You don't have ${permission} permission for ${module}.`;

    this.errorSubject.next({
      module,
      permission,
      message,
      timestamp: new Date()
    });

    // Auto-clear after 5 seconds
    setTimeout(() => this.clearError(), 5000);
  }

  /**
   * Clear the current error
   */
  clearError(): void {
    this.errorSubject.next(null);
  }
}
