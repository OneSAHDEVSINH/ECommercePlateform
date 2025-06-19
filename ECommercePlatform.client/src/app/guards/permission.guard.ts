import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router, UrlTree } from '@angular/router';
import { Observable, map, catchError, of } from 'rxjs';
import { AuthService } from '../services/auth/auth.service';
import { AuthorizationService } from '../services/authorization/authorization.service';
import { PermissionType } from '../models/role.model';

@Injectable({
  providedIn: 'root'
})
export class PermissionGuard implements CanActivate {
  constructor(
    private authService: AuthService,
    private authorizationService: AuthorizationService,
    private router: Router
  ) { }

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {

    // Step 1: Check if the user is authenticated
    if (!this.authService.isAuthenticated()) {
      return this.router.createUrlTree(['/admin/login'], {
        queryParams: { returnUrl: state.url }
      });
    }

    // Step 2: Check if user has any role (per your requirement)
    if (!this.authService.hasAnyRole()) {
      // User is authenticated but has no roles
      console.warn('User has no assigned roles');
      return this.router.createUrlTree(['/admin/login'], {
        queryParams: {
          returnUrl: state.url,
          error: 'no-roles'
        }
      });
    }

    // Step 3: Check if route is exempt from permission checks
    if (route.data['exempt'] === true) {
      return true;
    }

    // Step 4: Super admin bypass
    if (this.authorizationService.isAdmin()) {
      return true;
    }

    // Step 5: Check specific permissions
    const moduleRoute = route.data['moduleRoute'] || this.getModuleRouteFromUrl(state.url);
    const requiredPermission = route.data['permission'] as PermissionType || PermissionType.View;

    // For routes without module specification, allow access
    if (!moduleRoute) {
      return true;
    }

    // Check permission with the authorization service
    return this.authorizationService.checkPermission(moduleRoute, requiredPermission).pipe(
      map(hasPermission => {
        if (hasPermission) {
          return true;
        }

        // No permission, redirect to dashboard
        console.warn(`Access denied to ${moduleRoute}: Missing ${requiredPermission} permission`);
        return this.router.createUrlTree(['/admin/dashboard'], {
          queryParams: {
            accessDenied: 'true',
            module: moduleRoute,
            permission: requiredPermission
          }
        });
      }),
      catchError(error => {
        console.error('Permission check failed:', error);
        return of(this.router.createUrlTree(['/admin/dashboard']));
      })
    );
  }

  private getModuleRouteFromUrl(url: string): string {
    const segments = url.split('/').filter(segment => segment.length > 0);

    if (segments.length < 2) {
      return '';
    }

    // Return the module name (second segment after 'admin')
    return segments[1];
  }
}
