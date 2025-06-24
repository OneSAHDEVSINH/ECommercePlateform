import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router, UrlTree } from '@angular/router';
import { Observable, map, catchError, of, forkJoin, mergeMap } from 'rxjs';
import { AuthService } from '../services/auth/auth.service';
import { AuthorizationService } from '../services/authorization/authorization.service';
import { PermissionType } from '../models/role.model';
import { PermissionNotificationService } from '../services/general/permission-notification.service';

@Injectable({
  providedIn: 'root'
})
export class PermissionGuard implements CanActivate {
  constructor(
    private authService: AuthService,
    public authorizationService: AuthorizationService,
    private permissionNotificationService: PermissionNotificationService,
    private router: Router
  ) { }

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {

    // Special case: always allow access to the access-denied page
    if (state.url.includes('/admin/access-denied')) {
      return true;
    }

    // Steps 1-4 remain unchanged...
    if (!this.authService.isAuthenticated()) {
      return this.router.createUrlTree(['/admin/login'], {
        queryParams: { returnUrl: state.url }
      });
    }

    if (!this.authService.hasAnyRole()) {
      return this.router.createUrlTree(['/admin/login'], {
        queryParams: {
          returnUrl: state.url,
          error: 'no-roles'
        }
      });
    }

    if (route.data['exempt'] === true) {
      return true;
    }

    if (this.authorizationService.isAdmin()) {
      return true;
    }

    // Step 5: Check specific permissions
    const moduleRoute = route.data['moduleRoute'] || this.getModuleRouteFromUrl(state.url);
    const requiredPermission = route.data['permission'] as PermissionType || PermissionType.View;

    if (!moduleRoute) {
      return true;
    }

    // If requiredPermission is an array, check if user has ANY of them
    if (Array.isArray(requiredPermission)) {
      return this.checkForAnyPermission(moduleRoute, requiredPermission, state.url);
    }

    // Check primary permission first
    return this.authorizationService.checkPermission(moduleRoute, requiredPermission).pipe(
      mergeMap(hasPermission => {
        if (hasPermission) {
          return of(true);
        }

        // If primary permission check failed and it was View permission,
        // check if user has any other permissions
        if (requiredPermission === PermissionType.View) {
          return this.checkForAnyPermission(moduleRoute, [
            //PermissionType.Add,
            //PermissionType.Edit,
            PermissionType.AddEdit,
            PermissionType.Delete
          ], state.url);
        }

        // Otherwise show access denied
        //console.warn(`Access denied to ${moduleRoute}: Missing ${this.getReadablePermissionType(requiredPermission)} permission`);

        //if (moduleRoute === 'dashboard') {
        //  return of(this.router.createUrlTree(['/admin/login'], {
        //    queryParams: {
        //      accessDenied: 'true',
        //      module: moduleRoute,
        //      permission: this.getReadablePermissionType(requiredPermission)
        //    }
        //  }));
        //}

        //return of(this.router.createUrlTree(['/admin/access-denied'], {
        //  queryParams: {
        //    accessDenied: 'true',
        //    module: moduleRoute,
        //    permission: this.getReadablePermissionType(requiredPermission),
        //    returnUrl: state.url
        //  }
        //}));
        this.permissionNotificationService.showPermissionError(
          moduleRoute,
          this.getReadablePermissionType(requiredPermission)
        );
        return of(false);
      }),
      catchError(error => {
        console.error('Permission check failed:', error);
        return of(this.router.createUrlTree(['/admin/access-denied'], {
          queryParams: {
            permissionError: 'true',
            errorMessage: 'Error checking permissions'
          }
        }));
      })
    );
  }

  // Helper method to check if user has ANY of the given permissions
  //private checkForAnyPermission(moduleRoute: string, permissions: PermissionType[], returnUrl: string): Observable<boolean | UrlTree> {
  //  const permissionChecks$ = permissions.map(permission =>
  //    this.authorizationService.checkPermission(moduleRoute, permission)
  //  );

  //  // If there are no permissions to check, deny access
  //  if (permissionChecks$.length === 0) {
  //    return of(this.router.createUrlTree(['/admin/access-denied'], {
  //      queryParams: {
  //        accessDenied: 'true',
  //        module: moduleRoute,
  //        permission: 'any',
  //        returnUrl: returnUrl
  //      }
  //    }));
  //  }

  //  return forkJoin(permissionChecks$).pipe(
  //    map(results => {
  //      const hasAnyPermission = results.some(result => result === true);

  //      if (hasAnyPermission) {
  //        return true;
  //      }

  //      return this.router.createUrlTree(['/admin/access-denied'], {
  //        queryParams: {
  //          accessDenied: 'true',
  //          module: moduleRoute,
  //          permission: 'any',
  //          returnUrl: returnUrl
  //        }
  //      });
  //    }),
  //    catchError(error => {
  //      console.error('Permission checks failed:', error);
  //      return of(this.router.createUrlTree(['/admin/access-denied'], {
  //        queryParams: {
  //          permissionError: 'true',
  //          errorMessage: 'Error checking permissions'
  //        }
  //      }));
  //    })
  //  );
  //}

  private checkForAnyPermission(moduleRoute: string, permissions: PermissionType[], returnUrl: string): Observable<boolean | UrlTree> {
    // First check if this module exists/is accessible to the user at all
    return this.authorizationService.hasAnyPermission(moduleRoute).pipe(
      mergeMap(hasAnyModulePermission => {
        // If user has no permissions at all for this module, redirect to access denied
        if (!hasAnyModulePermission) {
          return of(this.router.createUrlTree(['/admin/access-denied'], {
            queryParams: {
              accessDenied: 'true',
              module: moduleRoute,
              permission: 'any',
              returnUrl: returnUrl
            }
          }));
        }

        // Otherwise check for the specific permissions requested
        const permissionChecks$ = permissions.map(permission =>
          this.authorizationService.checkPermission(moduleRoute, permission)
        );

        return forkJoin(permissionChecks$).pipe(
          map(results => {
            const hasAnyPermission = results.some(result => result === true);

            if (hasAnyPermission) {
              return true;
            } else {
              // User has some permissions for this module but not the required ones
              // Show a notification instead of redirecting
              this.permissionNotificationService.showPermissionError(
                moduleRoute,
                'appropriate',
                `You need additional permissions to access this ${moduleRoute} feature`
              );
              return false;
            }
          })
        );
      }),
      catchError(error => {
        console.error('Permission checks failed:', error);
        return of(this.router.createUrlTree(['/admin/access-denied'], {
          queryParams: {
            permissionError: 'true',
            errorMessage: 'Error checking permissions'
          }
        }));
      })
    );
  }

  // Other helper methods remain the same
  private getReadablePermissionType(type: PermissionType): string {
    switch (type) {
      case PermissionType.View: return 'View';
      //case PermissionType.Add: return 'Add';
      //case PermissionType.Edit: return 'Edit';
      case PermissionType.AddEdit: return 'AddEdit';
      case PermissionType.Delete: return 'Delete';
      default: return 'access';
    }
  }

  private getModuleRouteFromUrl(url: string): string {
    const segments = url.split('/').filter(segment => segment.length > 0);
    return segments.length < 2 ? '' : segments[1];
  }
}
