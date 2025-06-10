//import { Injectable } from '@angular/core';
//import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
//import { PermissionService } from '../services/authorization/permission.service';

//@Injectable({ providedIn: 'root' })
//export class PermissionGuard implements CanActivate {
//  constructor(private permissionService: PermissionService, private router: Router) { }

//  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
//    const module = route.data['module'];
//    const permission = route.data['permission'];
//    if (this.permissionService.hasPermission(module, permission)) {
//      return true;
//    }
//    // Optionally redirect to a not-authorized page
//    //this.router.navigate(['/not-authorized']);
//    return false;
//  }
//}

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
    // First check if the user is authenticated
    if (!this.authService.isAuthenticated()) {
      // Redirect to login with return URL
      return this.router.createUrlTree(['/admin/login'], {
        queryParams: { returnUrl: state.url }
      });
    }

    // Check if the user is an admin
    if (!this.authService.isAdmin()) {
      // Not an admin, redirect back to login with access denied message
      this.router.navigate(['/admin/login']);
      return false;
    }

    // Check if route is exempt from permission checks (like dashboard)
    if (route.data['exempt'] === true) {
      return true;
    }

    // Get module route and required permission from route data
    const moduleRoute = route.data['moduleRoute'] || this.getModuleRouteFromUrl(state.url);
    const requiredPermission = route.data['permission'] as PermissionType || PermissionType.VIEW;

    // If this is an admin-only section and user is super admin, allow without further checks
    if (route.data['adminOnly'] && this.authService.isSuperAdmin()) {
      return true;
    }

    // Check permission with the authorization service
    return this.authorizationService.checkPermission(moduleRoute, requiredPermission).pipe(
      map(hasPermission => {
        if (hasPermission) {
          return true;
        }

        // No permission, redirect to dashboard with notification
        console.warn(`Access denied to ${moduleRoute}: Missing ${requiredPermission} permission`);
        return this.router.createUrlTree(['/admin/dashboard']);
      }),
      catchError(error => {
        console.error('Permission check failed:', error);
        // On error, redirect to dashboard
        return of(this.router.createUrlTree(['/admin/dashboard']));
      })
    );
  }

  private getModuleRouteFromUrl(url: string): string {
    // Extract the module name from the URL
    // Format: /admin/moduleName or /admin/moduleName/subpath
    const segments = url.split('/').filter(segment => segment.length > 0);

    // Handle the case of /admin or /admin/
    if (segments.length < 2) {
      return '';
    }

    // For /admin/moduleName, return moduleName
    return segments[1];
  }
}
