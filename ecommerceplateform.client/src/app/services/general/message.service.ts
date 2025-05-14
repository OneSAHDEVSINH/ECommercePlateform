// src/app/services/message.service.ts
import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

export interface Message {
  type: 'success' | 'error' | 'info' | 'warning';
  text: string;
}

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  private messageSource = new BehaviorSubject<Message | null>(null);
  currentMessage = this.messageSource.asObservable();
  private timeout: any;
  private defaultDuration = 2500; // 2.5 seconds default

  constructor() { }

  showMessage(message: Message, duration: number = this.defaultDuration): void {
    // Clear any existing timeout
    this.clearTimeout();

    // Set the new message
    this.messageSource.next(message);

    // Set timeout to clear the message
    this.timeout = setTimeout(() => {
      this.clearMessage();
    }, duration);
  }

  clearMessage(): void {
    this.messageSource.next(null);
    this.clearTimeout();
  }

  private clearTimeout(): void {
    if (this.timeout) {
      clearTimeout(this.timeout);
      this.timeout = null;
    }
  }
}
