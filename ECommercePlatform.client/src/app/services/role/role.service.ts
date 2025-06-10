import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Role, Module, Permission, RolePermissionRequest, UserRoleAssignment, ModulePermission } from '../../models/role.model';
import { PagedResponse, PagedRequest } from '../../models/pagination.model';

@Injectable({
  providedIn: 'root'
})
export class RoleService {
  private apiUrl = `${environment.apiUrl}/roles`;
  private moduleApiUrl = `${environment.apiUrl}/modules`;

  constructor(private http: HttpClient) { }

  // Role methods
  getRoles(): Observable<Role[]> {
    return this.http.get<Role[]>(this.apiUrl);
  }

  getPagedRoles(request: PagedRequest): Observable<PagedResponse<Role>> {
    return this.http.get<PagedResponse<Role>>(`${this.apiUrl}/paged`, { params: { ...request as any } });
  }

  getRole(id: string): Observable<Role> {
    return this.http.get<Role>(`${this.apiUrl}/${id}`);
  }

  createRole(role: Role): Observable<Role> {
    return this.http.post<Role>(this.apiUrl, role);
  }

  updateRole(id: string, role: Role): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, role);
  }

  deleteRole(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  // Module methods
  getModules(): Observable<Module[]> {
    return this.http.get<Module[]>(this.moduleApiUrl);
  }

  // Permission methods
  assignPermissionsToRole(request: RolePermissionRequest): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/permissions`, request);
  }

  getRolePermissions(roleId: string): Observable<ModulePermission[]> {
    return this.http.get<ModulePermission[]>(`${this.apiUrl}/${roleId}/permissions`);
  }

  // User-Role assignment
  assignRolesToUser(assignment: UserRoleAssignment): Observable<void> {
    return this.http.post<void>(`${environment.apiUrl}/users/roles`, assignment);
  }

  getUserRoles(userId: string): Observable<Role[]> {
    return this.http.get<Role[]>(`${environment.apiUrl}/users/${userId}/roles`);
  }
}
