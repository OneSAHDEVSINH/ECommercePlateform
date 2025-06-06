import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-page-not-found',
  templateUrl: './page-not-found.component.html',
  styleUrls: ['./page-not-found.component.scss'],
  standalone: true,
  imports: [CommonModule, RouterModule]
})
export class PageNotFoundComponent implements OnInit {
  originalUrl: string = '';
  hasMalformedUrl: boolean = false;

  constructor(
    private router: Router,
    private route: ActivatedRoute
  ) { }

  ngOnInit(): void {
    // Get the current URL
    const currentUrl = window.location.href;
    this.originalUrl = currentUrl;

    // Check if there are any malformed URL parameters
    if (currentUrl.includes('%F') ||
      currentUrl.includes('%f') ||
      currentUrl.match(/%[0-9A-F]([^0-9A-F]|$)/i)) {
      this.hasMalformedUrl = true;

      // Try to fix and redirect if this is a known pattern
      const fixedUrl = this.fixMalformedUrl(currentUrl);
      if (fixedUrl !== currentUrl) {
        // Short delay before redirect to allow the component to render
        setTimeout(() => {
          window.location.href = fixedUrl;
        }, 5000);
      }
    }

    // Check if there's a sanitized returnUrl that we should redirect to
    const returnUrl = this.getValidReturnUrl();
    if (returnUrl !== null && returnUrl !== '/404') {
      setTimeout(() => {
        this.router.navigateByUrl(returnUrl);
      }, 3000);
    }
  }

  private getValidReturnUrl(): string | null {
    const queryReturnUrl = this.route.snapshot.queryParams['returnUrl'];
    if (!queryReturnUrl) return null;

    try {
      // Try to sanitize the URL before decoding
      let sanitizedUrl = queryReturnUrl
        .replace(/%F([^0-9A-F]|$)/gi, '%2F$1')
        .replace(/%([0-9A-F])([^0-9A-F]|$)/gi, '%0$1$2')
        .replace(/%(?![0-9A-F]{2})/gi, '%25');

      // Now try to decode
      const decodedUrl = decodeURIComponent(sanitizedUrl);

      // Security checks
      if (!decodedUrl.startsWith('/')) {
        return null;
      }

      if (decodedUrl.includes('//') || decodedUrl.includes('\\')) {
        return null;
      }

      return decodedUrl;
    } catch (e) {
      console.error('Failed to parse returnUrl:', e);
      return null;
    }
  }

  private fixMalformedUrl(url: string): string {
    try {
      // Fix common encoding issues
      return url
        .replace(/%F([^0-9A-F]|$)/gi, '%2F$1') // Fix %F -> %2F (slash)
        .replace(/%([0-9A-F])([^0-9A-F]|$)/gi, '%0$1$2') // Fix single digit hex
        .replace(/%(?![0-9A-F]{2})/gi, '%25') // Replace standalone % with %25
        .replace(/%([^0-9A-F]{2})/gi, '%25$1') // Fix illegal hex sequences
        .replace(/%$/, '%25'); // Fix trailing %
    } catch (e) {
      // If any error occurs during fixing, return the original URL
      console.error('Error fixing URL:', e);
      return url;
    }
  }
}
