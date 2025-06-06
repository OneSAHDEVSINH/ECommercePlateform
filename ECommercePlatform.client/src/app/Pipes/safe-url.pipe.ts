// src/app/pipes/safe-url.pipe.ts
import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'safeUrl'
})
export class SafeUrlPipe implements PipeTransform {
  transform(value: string, fallback: string = '/'): string {
    if (!value) return fallback;

    try {
      // Try to decode to verify it's valid
      decodeURIComponent(value);
      return value;
    } catch {
      // If decoding fails, try to fix common issues
      try {
        // Replace common malformed patterns
        const fixed = value
          .replace(/%F/g, '%2F')  // Fix missing 2 in %2F
          .replace(/%2f/g, '%2F') // Normalize case
          .replace(/%([0-9A-F])([^0-9A-F])/gi, '%0$1$2'); // Fix single digit hex

        decodeURIComponent(fixed);
        return fixed;
      } catch {
        return fallback;
      }
    }
  }
}
