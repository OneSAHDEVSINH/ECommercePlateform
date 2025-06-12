import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Role, Module, Permission, RolePermissionRequest, UserRoleAssignment, ModulePermission } from '../../models/role.model';
import { PagedResponse, PagedRequest } from '../../models/pagination.model';

@Injectable({
  providedIn: 'root'
})
export class RoleService {
  private apiUrl = `${environment.apiUrl}/role`;
  private moduleApiUrl = `${environment.apiUrl}/module`;
  private userApiUrl = `${environment.apiUrl}/user`;

  constructor(private http: HttpClient) { }

  // Role methods
  getRoles(): Observable<Role[]> {
    return this.http.get<Role[]>(this.apiUrl)
      .pipe(catchError(this.handleError));
  }

  getPagedRoles(request: PagedRequest): Observable<PagedResponse<Role>> {
    return this.http.get<PagedResponse<Role>>(`${this.apiUrl}/paged`, {
      params: { ...request as any }
    }).pipe(catchError(this.handleError));
  }

  getRole(id: string): Observable<Role> {
    return this.http.get<Role>(`${this.apiUrl}/${id}`)
      .pipe(catchError(this.handleError));
  }

  createRole(role: any): Observable<Role> {
    return this.http.post<Role>(this.apiUrl, role)
      .pipe(catchError(this.handleError));
  }

  updateRole(id: string, role: any): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, { ...role, id })
      .pipe(catchError(this.handleError));
  }

  deleteRole(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`)
      .pipe(catchError(this.handleError));
  }

  // Module methods
  getModules(): Observable<Module[]> {
    return this.http.get<Module[]>(this.moduleApiUrl)
      .pipe(catchError(this.handleError));
  }

  // Permission methods
  assignPermissionsToRole(request: RolePermissionRequest): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/permissions`, request)
      .pipe(catchError(this.handleError));
  }

  getRolePermissions(roleId: string): Observable<ModulePermission[]> {
    return this.http.get<ModulePermission[]>(`${this.apiUrl}/${roleId}/permissions`)
      .pipe(catchError(this.handleError));
  }

  // User-Role assignment
  assignRolesToUser(assignment: UserRoleAssignment): Observable<void> {
    return this.http.post<void>(`${this.userApiUrl}/roles`, assignment)
      .pipe(catchError(this.handleError));
  }

  getUserRoles(userId: string): Observable<Role[]> {
    return this.http.get<Role[]>(`${this.userApiUrl}/${userId}/roles`)
      .pipe(catchError(this.handleError));
  }

  getUsersByRole(roleId: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.userApiUrl}/by-role/${roleId}`)
      .pipe(catchError(this.handleError));
  }

  private handleError(error: any) {
    console.error('An error occurred', error);
    return throwError(() => error);
  }
}
