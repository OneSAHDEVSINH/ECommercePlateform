import { Injectable } from '@angular/core';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class SafeNavigationService {
  constructor(private router: Router) { }

  navigateToUrl(url: string): void {
    try {
      // Try to decode to check if it's valid
      const decodedUrl = decodeURIComponent(url);
      this.router.navigateByUrl(decodedUrl);
    } catch (e) {
      console.error('Malformed URL detected:', url);
      // Navigate to 404 with information about the malformed URL
      this.router.navigate(['/404'], {
        queryParams: {
          malformed: 'true',
          originalUrl: url
        }
      });
    }
  }

  navigateWithReturnUrl(path: string, returnUrl: string): void {
    try {
      // Validate return URL
      const validReturnUrl = this.validateReturnUrl(returnUrl);

      this.router.navigate([path], {
        queryParams: {
          returnUrl: validReturnUrl
        }
      });
    } catch (e) {
      // If validation fails, navigate without return URL
      this.router.navigate([path]);
    }
  }

  private validateReturnUrl(url: string): string {
    if (!url) return '/';

    try {
      // Remove any malformed encoding patterns
      const cleanUrl = url
        .replace(/%F/g, '%2F')
        .replace(/%([0-9A-F])([^0-9A-F])/gi, '%0$1$2');

      // Test if it decodes properly
      decodeURIComponent(cleanUrl);

      return cleanUrl;
    } catch {
      return '/';
    }
  }
}
