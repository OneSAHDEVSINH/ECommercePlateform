import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth/auth.service';
import { SafeNavigationService } from '../services/safe-navigation.service';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);
  const safeNav = inject(SafeNavigationService);

  if (authService.isAuthenticated() && authService.isAdmin()) {
    return true;
  }

  //// Store the attempted URL for redirecting
  //router.navigate(['/admin/login'], {
  //  queryParams: { returnUrl: state.url }
  //});

  // Ensure URL is properly encoded
  const safeUrl = state.url.split('?')[0]; // Remove query params
  const queryPart = state.url.split('?')[1]; // Get query params if any

  // Store the attempted URL for redirecting
  //router.navigate(['/admin/login'], {
  //  queryParams: {
  //    returnUrl: safeUrl // Pass only the path without encoding issues
  //  }
  //});

  safeNav.navigateWithReturnUrl('/admin/login', state.url);

  return false;
}; 
