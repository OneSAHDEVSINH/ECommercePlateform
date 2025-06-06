import { Injectable } from '@angular/core';
import { AuthService } from '../auth/auth.service';
import { Observable, of, map, catchError } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { PermissionType } from '../../models/role.model';

@Injectable({
  providedIn: 'root'
})
export class AuthorizationService {
  private apiUrl = `${environment.apiUrl}/api/authorization`;
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
      catchError(() => {
        this.cachedPermissions[cacheKey] = false;
        return of(false);
      })
    );
  }

  hasViewPermission(moduleRoute: string): Observable<boolean> {
    return this.checkPermission(moduleRoute, PermissionType.View);
  }

  hasCreatePermission(moduleRoute: string): Observable<boolean> {
    return this.checkPermission(moduleRoute, PermissionType.Create);
  }

  hasEditPermission(moduleRoute: string): Observable<boolean> {
    return this.checkPermission(moduleRoute, PermissionType.Edit);
  }

  hasDeletePermission(moduleRoute: string): Observable<boolean> {
    return this.checkPermission(moduleRoute, PermissionType.Delete);
  }

  clearCache(): void {
    this.cachedPermissions = {};
  }
}
