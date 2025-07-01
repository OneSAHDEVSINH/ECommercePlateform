import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, throwError, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import { User } from '../../models/user.model';
import { PagedResponse, PagedRequest } from '../../models/pagination.model';
import { AuthorizationService } from '../authorization/authorization.service';

@Injectable({
  providedIn: 'root'
})

export class UserService {
  private apiUrl = `${environment.apiUrl}/user`;

  constructor(private http: HttpClient, private authorizationService: AuthorizationService) { }

  getUsers(): Observable<User[]> {
    return this.http.get<User[]>(this.apiUrl)
      .pipe(catchError(this.handleError));
  }

  getPagedUsers(request: PagedRequest, roleId?: string, activeOnly?: boolean, includeRoles: boolean = true): Observable<PagedResponse<User>> {
    return this.http.get<PagedResponse<User>>(`${this.apiUrl}/paged`, {
      params: this.createParams(request, roleId, activeOnly, includeRoles)
    }).pipe(
      map(response => {
        // Filter out ONLY the default SuperAdmin account (admin@admin.com) for non-admin users
        if (response && response.items && !this.authorizationService.isAdmin()) {
          response.items = response.items.filter(user =>
            // Only hide the default SuperAdmin account by email
            user.email?.toLowerCase() !== 'admin@admin.com'
          );
        }
        return response;
      }),
      catchError(this.handleError)
    );
  }

  // Helper method to create HTTP parameters
  private createParams(request: PagedRequest, roleId?: string, activeOnly?: boolean, includeRoles: boolean = true): any {
    let params: any = {
      ...request,
      includeRoles: includeRoles
    };

    if (roleId && roleId !== 'all') {
      params.roleId = roleId;
    }

    if (activeOnly !== undefined) {
      params.activeOnly = activeOnly;
    }

    return params;
  }

  getUserById(id: string): Observable<User> {
    return this.http.get<User>(`${this.apiUrl}/${id}`)
      .pipe(catchError(this.handleError));
  }

  getUserByEmail(email: string): Observable<User> {
    return this.http.get<User>(`${this.apiUrl}/by-email`, {
      params: { email }
    }).pipe(catchError(this.handleError));
  }

  createUser(user: any): Observable<User> {
    return this.http.post<User>(this.apiUrl, user)
      .pipe(catchError(this.handleError));
  }

  updateUser(id: string, user: any): Observable<User> {
    return this.http.put<User>(`${this.apiUrl}/${id}`, { ...user, id })
      .pipe(catchError(this.handleError));
  }

  deleteUser(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`)
      .pipe(catchError(this.handleError));
  }

  getUserWithRoles(id: string): Observable<User> {
    return this.http.get<User>(`${this.apiUrl}/${id}/roles`)
      .pipe(catchError(this.handleError));
  }

  assignRolesToUser(userId: string, roleIds: string[]): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${userId}/roles`, roleIds)
      .pipe(catchError(this.handleError));
  }

  private handleError(error: any) {
    console.error('User service error:', error);
    return throwError(() => error);
  }
}
