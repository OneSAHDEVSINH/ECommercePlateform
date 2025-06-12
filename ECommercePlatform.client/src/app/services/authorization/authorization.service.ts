import { Injectable } from '@angular/core';
import { AuthService } from '../auth/auth.service';
import { Observable, of, map, catchError, shareReplay } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { PermissionType } from '../../models/role.model';

@Injectable({
  providedIn: 'root'
})
export class AuthorizationService {
  private apiUrl = `${environment.apiUrl}/authorization`;
  private cachedPermissions: Record<string, boolean> = {};

  constructor(
    private authService: AuthService,
    private http: HttpClient
  ) { }

  checkPermission(moduleRoute: string, permissionType: PermissionType): Observable<boolean> {
    const cacheKey = `${moduleRoute}-${permissionType}`;

    // Return cached result if available
    if (this.cachedPermissions[cacheKey] !== undefined) {
      return of(this.cachedPermissions[cacheKey]);
    }

    // For super admin, always allow access
    if (this.authService.isSuperAdmin()) {
      this.cachedPermissions[cacheKey] = true;
      return of(true);
    }

    return this.http.get<boolean>(`${this.apiUrl}/check`, {
      params: {
        moduleRoute,
        permissionType
      }
    }).pipe(
      map(hasPermission => {
        this.cachedPermissions[cacheKey] = hasPermission;
        return hasPermission;
      }),
      shareReplay(1),
      catchError(error => {
        console.warn(`Permission check error for ${moduleRoute}-${permissionType}:`, error);
        this.cachedPermissions[cacheKey] = false;
        return of(false);
      })
    );
  }

  hasViewPermission(moduleRoute: string): Observable<boolean> {
    return this.checkPermission(moduleRoute, PermissionType.VIEW);
  }

  hasCreatePermission(moduleRoute: string): Observable<boolean> {
    return this.checkPermission(moduleRoute, PermissionType.ADD);
  }

  hasEditPermission(moduleRoute: string): Observable<boolean> {
    return this.checkPermission(moduleRoute, PermissionType.EDIT);
  }

  hasDeletePermission(moduleRoute: string): Observable<boolean> {
    return this.checkPermission(moduleRoute, PermissionType.DELETE);
  }

  clearCache(): void {
    this.cachedPermissions = {};
  }
}
