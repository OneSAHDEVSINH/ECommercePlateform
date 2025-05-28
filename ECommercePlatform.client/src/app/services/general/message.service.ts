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
    
    // Add global scroll to top
    this.scrollToTop();

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

  // New method to handle scrolling
  scrollToTop(): void {
    setTimeout(() => {
      try {
        // Try multiple approaches for maximum compatibility
        window.scrollTo(0, 0);
        window.scrollTo({ top: 0, behavior: 'smooth' });
        
        // Browser compatibility fixes
        document.body.scrollTop = 0; // For Safari
        document.documentElement.scrollTop = 0; // For Chrome, Firefox, IE and Opera
        
        // Find and scroll specific content containers that might be in a scrollable state
        const mainContent = document.querySelector('.main-content');
        if (mainContent) {
          (mainContent as HTMLElement).scrollTop = 0;
        }
        
        const contentArea = document.querySelector('.content');
        if (contentArea) {
          (contentArea as HTMLElement).scrollTop = 0;
        }
      } catch (err) {
        console.error('Error during scroll operation:', err);
      }
    }, 100);
  }
}
