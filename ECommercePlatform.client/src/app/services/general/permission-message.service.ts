import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

export interface PermissionMessage {
  module: string;
  permission: string;
  error?: string;
  isError: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class PermissionMessageService {
  private messageSubject = new BehaviorSubject<PermissionMessage | null>(null);
  public message$: Observable<PermissionMessage | null> = this.messageSubject.asObservable();

  showAccessDenied(module: string, permission: string): void {
    this.messageSubject.next({
      module,
      permission,
      isError: true
    });

    // Auto-clear message after 8 seconds
    setTimeout(() => this.clearMessage(), 8000);
  }

  showPermissionError(error: string): void {
    this.messageSubject.next({
      module: '',
      permission: '',
      error,
      isError: true
    });

    // Auto-clear message after 8 seconds
    setTimeout(() => this.clearMessage(), 8000);
  }

  clearMessage(): void {
    this.messageSubject.next(null);
  }
}
