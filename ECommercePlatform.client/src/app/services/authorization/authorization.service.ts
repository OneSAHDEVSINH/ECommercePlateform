import { Injectable } from '@angular/core';
import { AuthService } from '../auth/auth.service';
import { Observable, of, map, catchError, shareReplay, BehaviorSubject, tap, forkJoin } from 'rxjs';
import { HttpClient } from '@angular/common/http';
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

    // Check memory cache first
    if (this.permissionsCache.has(cacheKey)) {
      return of(this.permissionsCache.get(cacheKey)!);
    }

    // Check local permissions
    const hasLocalPermission = this.hasPermission(moduleRoute, permissionType);
    if (hasLocalPermission) {
      this.permissionsCache.set(cacheKey, true);
      this.permissionCacheExpiry.set(cacheKey, Date.now() + this.CACHE_MAX_AGE);
      return of(true);
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

  // Optionally, helper methods for better cache management
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

    // Use cached permissions from the initial load
    const cachedPermissions = this.getCachedPermissions();
    const permissionKey = `${moduleRoute}:${this.permissionTypeToString(permission)}`;

    return cachedPermissions.includes(permissionKey);
  }

  private getCachedPermissions(): string[] {
    const cachedPermissions: string[] = [];
    this.permissionsCache.forEach((hasPermission, key) => {
      if (hasPermission) {
        // Convert from cache key format (moduleRoute-permissionType) to permission string format (moduleRoute:permissionType)
        const [moduleRoute, permType] = key.split('-');
        if (moduleRoute && permType) {
          cachedPermissions.push(`${moduleRoute}:${permType}`);
        }
      }
    });
    return cachedPermissions;
  }

  private permissionTypeToString(permission: PermissionType): string {
    switch (permission) {
      case PermissionType.View: return 'View';
      case PermissionType.AddEdit: return 'AddEdit';
      case PermissionType.Delete: return 'Delete';
      default: return '';
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
