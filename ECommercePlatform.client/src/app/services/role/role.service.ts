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

  constructor(private http: HttpClient) { }

  getRoles(): Observable<Role[]> {
    return this.http.get<Role[]>(this.apiUrl)
      .pipe(catchError(this.handleError));
  }

  getPagedRoles(request: PagedRequest, activeOnly?: boolean): Observable<PagedResponse<Role>> {
    return this.http.get<PagedResponse<Role>>(`${this.apiUrl}/paged`, {
      params: this.createParams(request, activeOnly)
    }).pipe(catchError(this.handleError));
  }

  // Helper method to create HTTP parameters
  private createParams(request: PagedRequest, activeOnly?: boolean): any {
    let params: any = {
      ...request,
    };

    if (activeOnly !== undefined) {
      params.activeOnly = activeOnly;
    }

    return params;
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

  private handleError(error: any) {
    console.error('Role service error:', error);
    return throwError(() => error);
  }
}
