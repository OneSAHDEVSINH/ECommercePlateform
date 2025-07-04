import { Injectable } from '@angular/core';
import { AuthService } from '../auth/auth.service';
import { Observable, of, map, catchError, shareReplay, BehaviorSubject, tap, forkJoin, combineLatest } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from '../../../environments/environment';
import { PermissionType } from '../../models/role.model';

@Injectable({
  providedIn: 'root'
})
export class AuthorizationService {
  private apiUrl = `${environment.apiUrl}/authorization`;
  private permissionsCache = new Map<string, boolean>();
  private readonly CACHE_MAX_AGE = 5 * 60 * 1000; // 5 minutes in milliseconds
  private permissionCacheExpiry = new Map<string, number>();

  // Add reactive permission updates
  private permissionUpdateSubject = new BehaviorSubject<{ moduleRoute: string, permissionType: PermissionType } | null>(null);
  public permissionUpdated$ = this.permissionUpdateSubject.asObservable();

  // Global permission state change notifier
  private globalPermissionChangeSubject = new BehaviorSubject<boolean>(false);
  public globalPermissionChange$ = this.globalPermissionChangeSubject.asObservable();

  constructor(
    private authService: AuthService,
    private http: HttpClient,
    private router: Router

  ) {
    // Clear cache on auth state change
    this.authService.authStateChange$.subscribe(isLoggedIn => {
      if (!isLoggedIn) {
        this.permissionsCache.clear();
        this.permissionCacheExpiry.clear();
      }
    });

    // Listen to permission updates from auth service
    this.authService.permissions$.subscribe(() => {
      this.clearCache();
      this.notifyGlobalPermissionChange();
    });
  }

  isAdmin(): boolean {
    const currentUser = this.authService.getCurrentUser();
    if (!currentUser) return false;

    // Super admin bypass - always considered admin
    if (this.authService.isSuperAdmin()) return true;

    // Check for Admin role
    return currentUser.roles?.some(role => role.name === 'Admin') || false;
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
      case PermissionType.AddEdit:
        return modulePermission.canAddEdit;
      case PermissionType.Delete:
        return modulePermission.canDelete;
      default:
        return false;
    }
  }

  hasViewPermission(moduleRoute: string): boolean {
    return this.hasPermission(moduleRoute, PermissionType.View);
  }

  hasViewOrAddEditPermission(moduleRoute: string): boolean {
    return this.hasPermission(moduleRoute, PermissionType.View) ||
      this.hasPermission(moduleRoute, PermissionType.AddEdit);
  }

  hasDeleteOrAddEditPermission(moduleRoute: string): boolean {
    return this.hasPermission(moduleRoute, PermissionType.Delete) ||
      this.hasPermission(moduleRoute, PermissionType.AddEdit);
  }

  checkPermission(moduleRoute: string, permissionType: PermissionType): Observable<boolean> {
    // If user is superadmin, always return true without checking cache
    if (this.authService.isSuperAdmin()) {
      return of(true);
    }

    const cacheKey = `${moduleRoute}-${permissionType}`;

    // Check if cache exists and is still valid
    const cacheExpiry = this.permissionCacheExpiry.get(cacheKey);
    if (this.permissionsCache.has(cacheKey) && cacheExpiry && cacheExpiry > Date.now()) {
      return of(this.permissionsCache.get(cacheKey)!);
    }

    // Check local permissions first
    const hasLocalPermission = this.hasPermission(moduleRoute, permissionType);
    if (hasLocalPermission !== undefined) {
      this.permissionsCache.set(cacheKey, hasLocalPermission);
      this.permissionCacheExpiry.set(cacheKey, Date.now() + this.CACHE_MAX_AGE);
      return of(hasLocalPermission);
    }

    // If not found locally, check with server
    return this.http.get<{ hasPermission: boolean }>(`${this.apiUrl}/check`, {
      params: { moduleRoute, permissionType }
    }).pipe(
      map(result => result.hasPermission),
      tap(hasPermission => {
        this.permissionsCache.set(cacheKey, hasPermission);
        this.permissionCacheExpiry.set(cacheKey, Date.now() + this.CACHE_MAX_AGE);
      }),
      catchError(() => {
        console.error(`Error checking permission (${moduleRoute}:${permissionType}):`, Error);
        this.permissionsCache.set(cacheKey, false);
        this.permissionCacheExpiry.set(cacheKey, Date.now() + this.CACHE_MAX_AGE);
        return of(false);
      })
    );
  }

  // Reactive permission checking
  checkPermissionReactive(moduleRoute: string, permissionType: PermissionType): Observable<boolean> {
    return combineLatest([
      this.checkPermission(moduleRoute, permissionType),
      this.globalPermissionChange$
    ]).pipe(
      map(([hasPermission]) => hasPermission),
      shareReplay(1)
    );
  }

  getUserAccessibleModules(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/modules`);
  }

  clearCache(): void {
    this.permissionsCache.clear();
    this.permissionCacheExpiry.clear();

    // Optionally, might want to reload permissions from the server
    // This ensures the cache is refreshed with latest data
    if (this.authService.isAuthenticated()) {
      // Force a permission check to repopulate cache
      console.log('Permission cache cleared');
    }
  }

  // Enhanced cache management
  clearCacheForModule(moduleRoute: string): void {
    // Clear all cached permissions for a specific module
    const keysToDelete: string[] = [];
    this.permissionsCache.forEach((_, key) => {
      if (key.startsWith(`${moduleRoute}-`)) {
        keysToDelete.push(key);
      }
    });
    keysToDelete.forEach(key => {
      this.permissionsCache.delete(key);
      this.permissionCacheExpiry.delete(key);
    });

    // Notify components about permission changes for this module
    this.notifyPermissionUpdate(moduleRoute, PermissionType.View);
  }

  // Check if user has ANY permission for a module
  public hasAnyPermission(moduleRoute: string): Observable<boolean> {
    return forkJoin([
      this.checkPermission(moduleRoute, PermissionType.View),
      this.checkPermission(moduleRoute, PermissionType.AddEdit),
      this.checkPermission(moduleRoute, PermissionType.Delete)
    ]).pipe(
      map(results => results.some(result => result))
    );
  }

  // Check if user has ALL listed permissions for a module
  public hasAllPermissions(moduleRoute: string, permissions: PermissionType[]): Observable<boolean> {
    if (permissions.length === 0) return of(false);

    const checks = permissions.map(permission => this.checkPermission(moduleRoute, permission));
    return forkJoin(checks).pipe(
      map(results => results.every(result => result))
    );
  }

  // Synchronous permission check that uses cached permissions
  // Useful for rapid UI updates that don't need to wait for an API call
  public hasPermissionSync(moduleRoute: string, permission: PermissionType): boolean {
    if (this.isAdmin()) return true;

    // Use local permissions first, then cached
    const localHasPermission = this.hasPermission(moduleRoute, permission);
    if (localHasPermission !== undefined) {
      return localHasPermission;
    }

    // Fall back to cache
    const cacheKey = `${moduleRoute}-${permission}`;
    return this.permissionsCache.get(cacheKey) || false;
  }

  // Notification methods for reactive updates
  private notifyPermissionUpdate(moduleRoute: string, permissionType: PermissionType): void {
    this.permissionUpdateSubject.next({ moduleRoute, permissionType });
  }

  private notifyGlobalPermissionChange(): void {
    this.globalPermissionChangeSubject.next(true);
  }

  // Public method to trigger permission refresh
  public refreshPermissions(): void {
    this.clearCache();
    this.notifyGlobalPermissionChange();
  }

  private permissionTypeToString(permission: PermissionType): string {
    switch (permission) {
      case PermissionType.View: return 'View';
      case PermissionType.AddEdit: return 'AddEdit';
      case PermissionType.Delete: return 'Delete';
      default: return '';
    }
  }

  public checkAndRedirectOnPermissionLoss(moduleRoute: string, permissionType: PermissionType): void {
    if (!this.hasPermissionSync(moduleRoute, permissionType) && !this.isAdmin()) {
      // Use router to navigate immediately
      if (!this.router.url.includes('/admin/access-denied')) {
        this.router.navigate(['/admin/access-denied'], {
          queryParams: {
            accessDenied: 'true',
            module: moduleRoute,
            permission: this.getPermissionTypeString(permissionType),
            returnUrl: this.router.url
          }
        });
      }
    }
  }

  private getPermissionTypeString(type: PermissionType): string {
    switch (type) {
      case PermissionType.View: return 'View';
      case PermissionType.AddEdit: return 'AddEdit';
      case PermissionType.Delete: return 'Delete';
      default: return 'required';
    }
  }

  getCacheSize(): number {
    return this.permissionsCache.size;
  }

  isCached(moduleRoute: string, permissionType: PermissionType): boolean {
    const cacheKey = `${moduleRoute}-${permissionType}`;
    return this.permissionsCache.has(cacheKey);
  }
}
