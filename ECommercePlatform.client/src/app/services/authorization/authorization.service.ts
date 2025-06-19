import { Injectable } from '@angular/core';
import { AuthService } from '../auth/auth.service';
import { Observable, of, map, catchError, shareReplay, BehaviorSubject, tap } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { PermissionType } from '../../models/role.model';

@Injectable({
  providedIn: 'root'
})
export class AuthorizationService {
  private apiUrl = `${environment.apiUrl}/authorization`;
  private permissionsCache = new Map<string, boolean>();

  constructor(
    private authService: AuthService,
    private http: HttpClient
  ) {
    // Clear cache on auth state change
    this.authService.authStateChange$.subscribe(isLoggedIn => {
      if (!isLoggedIn) {
        this.permissionsCache.clear();
      }
    });
  }

  // Add this method
  isAdmin(): boolean {
    return this.authService.isSuperAdmin();
  }

  hasPermission(moduleRoute: string, permissionType: PermissionType): boolean {
    // Check if super admin
    if (this.authService.isSuperAdmin()) {
      return true;
    }

    // Get permissions from auth service
    const permissions = this.authService.getUserPermissions();

    // Find module permission
    const modulePermission = permissions.find(p =>
      p.moduleName?.toLowerCase() === moduleRoute.toLowerCase()
    );

    if (!modulePermission) return false;

    switch (permissionType) {
      case PermissionType.View:
        return modulePermission.canView;
      case PermissionType.Add:
        return modulePermission.canAdd;
      case PermissionType.Edit:
        return modulePermission.canEdit;
      case PermissionType.Delete:
        return modulePermission.canDelete;
      default:
        return false;
    }
  }

  checkPermission(moduleRoute: string, permissionType: PermissionType): Observable<boolean> {
    const cacheKey = `${moduleRoute}-${permissionType}`;

    // Check memory cache first
    if (this.permissionsCache.has(cacheKey)) {
      return of(this.permissionsCache.get(cacheKey)!);
    }

    // Check local permissions
    const hasLocalPermission = this.hasPermission(moduleRoute, permissionType);
    if (hasLocalPermission) {
      this.permissionsCache.set(cacheKey, true);
      return of(true);
    }

    // If not found locally, check with server
    return this.http.get<{ hasPermission: boolean }>(`${this.apiUrl}/check`, {
      params: { moduleRoute, permissionType }
    }).pipe(
      map(result => result.hasPermission),
      tap(hasPermission => {
        this.permissionsCache.set(cacheKey, hasPermission);
      }),
      catchError(() => {
        this.permissionsCache.set(cacheKey, false);
        return of(false);
      })
    );
  }

  getUserAccessibleModules(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/modules`);
  }

  clearCache(): void {
    this.permissionsCache.clear();

    // Optionally, you might want to reload permissions from the server
    // This ensures the cache is refreshed with latest data
    if (this.authService.isAuthenticated()) {
      // Force a permission check to repopulate cache
      console.log('Permission cache cleared');
    }
  }

  // Optionally, add these helper methods for better cache management
  clearCacheForModule(moduleRoute: string): void {
    // Clear all cached permissions for a specific module
    const keysToDelete: string[] = [];
    this.permissionsCache.forEach((_, key) => {
      if (key.startsWith(`${moduleRoute}-`)) {
        keysToDelete.push(key);
      }
    });
    keysToDelete.forEach(key => this.permissionsCache.delete(key));
  }

  getCacheSize(): number {
    return this.permissionsCache.size;
  }

  isCached(moduleRoute: string, permissionType: PermissionType): boolean {
    const cacheKey = `${moduleRoute}-${permissionType}`;
    return this.permissionsCache.has(cacheKey);
  }
}

//@Injectable({
//  providedIn: 'root'
//})
//export class AuthorizationService {
//  private apiUrl = `${environment.apiUrl}/authorization`;
//  private cachedPermissions: Record<string, boolean> = {};
//  private permissionsCache$ = new BehaviorSubject<string[]>([]);
//  private isAdmin$ = new BehaviorSubject<boolean>(false);
//  private permissionsLoaded = false;

//  constructor(
//    private authService: AuthService,
//    private http: HttpClient
//  ) {
//    // Clear permissions cache when user logs out
//    this.authService.authStateChange$.subscribe(isLoggedIn => {
//      if (!isLoggedIn) {
//        this.cachedPermissions = {};
//        this.permissionsCache$.next([]);
//        this.isAdmin$.next(false);
//        this.permissionsLoaded = false;
//      } else if (!this.permissionsLoaded) {
//        // Load permissions when user logs in
//        this.loadUserPermissions();
//      }
//    });
//  }

//  private loadUserPermissions() {
//    if (!this.authService.isAuthenticated()) return;

//    this.http.get<{ permissions: string[], isAdmin: boolean }>(`${this.apiUrl}/user-permissions`)
//      .pipe(
//        tap(result => {
//          this.permissionsCache$.next(result.permissions);
//          this.isAdmin$.next(result.isAdmin);
//          this.permissionsLoaded = true;

//          // Pre-populate the cache with known permissions
//          result.permissions.forEach(perm => {
//            const [moduleRoute, permType] = perm.split(':');
//            const cacheKey = `${moduleRoute}-${permType}`;
//            this.cachedPermissions[cacheKey] = true;
//          });
//        }),
//        catchError(error => {
//          console.error('Failed to load user permissions', error);
//          return of({ permissions: [], isAdmin: false });
//        })
//      )
//      .subscribe();
//  }

//  checkPermission(moduleRoute: string, permissionType: PermissionType): Observable<boolean> {
//    const cacheKey = `${moduleRoute}-${permissionType}`;

//    // Return cached result if available
//    if (this.cachedPermissions[cacheKey] !== undefined) {
//      return of(this.cachedPermissions[cacheKey]);
//    }

//    // For super admin, always allow access
//    if (this.isAdmin$.getValue()) {
//      this.cachedPermissions[cacheKey] = true;
//      return of(true);
//    }

//    // Check against stored permissions
//    const permissionKey = `${moduleRoute}:${permissionType}`;
//    const hasPermission = this.permissionsCache$.getValue().includes(permissionKey);

//    if (hasPermission) {
//      this.cachedPermissions[cacheKey] = true;
//      return of(true);
//    }

//    // If we don't have this permission in our cache and user isn't admin, check server
//    return this.http.get<boolean>(`${this.apiUrl}/check`, {
//      params: {
//        moduleRoute,
//        permissionType
//      }
//    }).pipe(
//      map(hasPermission => {
//        this.cachedPermissions[cacheKey] = hasPermission;
//        return hasPermission;
//      }),
//      shareReplay(1),
//      catchError(error => {
//        console.warn(`Permission check error for ${moduleRoute}-${permissionType}:`, error);
//        this.cachedPermissions[cacheKey] = false;
//        return of(false);
//      })
//    );
//  }

//  hasViewPermission(moduleRoute: string): Observable<boolean> {
//    return this.checkPermission(moduleRoute, PermissionType.View);
//  }

//  hasCreatePermission(moduleRoute: string): Observable<boolean> {
//    return this.checkPermission(moduleRoute, PermissionType.Add);
//  }

//  hasEditPermission(moduleRoute: string): Observable<boolean> {
//    return this.checkPermission(moduleRoute, PermissionType.Edit);
//  }

//  hasDeletePermission(moduleRoute: string): Observable<boolean> {
//    return this.checkPermission(moduleRoute, PermissionType.Delete);
//  }

//  clearCache(): void {
//    this.cachedPermissions = {};
//    this.permissionsLoaded = false;
//    this.loadUserPermissions();
//  }

//  get isAdmin(): boolean {
//    return this.isAdmin$.getValue();
//  }
//}
