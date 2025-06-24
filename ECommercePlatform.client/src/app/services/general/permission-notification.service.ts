import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { MessageService } from './message.service';

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
  private clearErrorTimeout: any;

  constructor(private messageService: MessageService) { }

  showPermissionError(module: string, permission: string, customMessage?: string): void {
    // Clear any existing error first
    this.clearError();

    const message = customMessage ||
      `You don't have ${permission} permission for ${module}.`;

    // Use the regular message service for console errors with a longer timeout
    this.messageService.showMessage(
      { type: 'warning', text: message },
      5000 // Longer timeout for permission errors
    );

    // Also track in our specialized service
    this.errorSubject.next({
      module,
      permission,
      message,
      timestamp: new Date()
    });

    // Auto-clear after 5 seconds - make sure this timeout works
    if (this.clearErrorTimeout) {
      clearTimeout(this.clearErrorTimeout);
    }

    this.clearErrorTimeout = setTimeout(() => {
      this.clearError();
    }, 5000);
  }

  clearError(): void {
    if (this.clearErrorTimeout) {
      clearTimeout(this.clearErrorTimeout);
    }
    this.errorSubject.next(null);
  }
}
