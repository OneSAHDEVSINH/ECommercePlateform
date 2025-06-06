// main.ts
import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { AppComponent } from './app/app.component';

// Sanitize URL before Angular starts
function sanitizeUrl() {
  const currentUrl = window.location.href;

  // Check for malformed URL encoding
  if (currentUrl.includes('%F') ||
    currentUrl.includes('%f') ||
    currentUrl.match(/%[0-9A-F]([^0-9A-F]|$)/i)) {

    // Redirect to 404 page with the malformed URL as a parameter
    const baseUrl = window.location.origin;
    const malformedPath = window.location.pathname + window.location.search;
    window.location.href = `${baseUrl}/404?malformed=${encodeURIComponent(malformedPath)}`;
    return false;
  }

  return true;
}

// Check URL before bootstrapping
if (sanitizeUrl()) {
  bootstrapApplication(AppComponent, appConfig)
    .catch((err) => console.error(err));
}
