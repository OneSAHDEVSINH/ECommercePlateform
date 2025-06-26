import { Injectable } from '@angular/core';
import { Router, NavigationError } from '@angular/router';
import { filter } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class NavigationErrorHandlerService {
  constructor(private router: Router) {
    this.setupNavigationErrorHandling();
  }

  private setupNavigationErrorHandling(): void {
    this.router.events
      .pipe(filter(event => event instanceof NavigationError))
      .subscribe((event: NavigationError) => {
        console.error('Navigation error:', event.error);

        // Check if it's a malformed URL error
        if (event.error?.message?.includes('URI malformed') ||
          event.url?.includes('%F')) {

          // Extract the malformed URL
          const malformedUrl = event.url || window.location.pathname;

          // Navigate to 404 page
          this.router.navigate(['/404'], {
            queryParams: {
              malformed: 'true',
              url: encodeURIComponent(malformedUrl)
            },
            replaceUrl: true
          });
        }
      });
  }

  initialize(): void {
    // This method is called to ensure the service is instantiated
  }
}
