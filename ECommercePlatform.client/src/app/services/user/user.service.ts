import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import { User } from '../../models/user.model';
import { PagedResponse, PagedRequest } from '../../models/pagination.model';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private apiUrl = `${environment.apiUrl}/user`;

  constructor(private http: HttpClient) { }

  getUsers(): Observable<User[]> {
    return this.http.get<User[]>(this.apiUrl)
      .pipe(catchError(this.handleError));
  }

  getPagedUsers(request: PagedRequest): Observable<PagedResponse<User>> {
    return this.http.get<PagedResponse<User>>(`${this.apiUrl}/paged`, {
      params: { ...request as any }
    }).pipe(catchError(this.handleError));
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

  updateUser(id: string, user: any): Observable<void> {
    // Ensure id is included in the request body
    const userData = { ...user, id };
    return this.http.put<void>(`${this.apiUrl}/${id}`, userData)
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

  // Fix: Send array directly, not wrapped in object
  assignRolesToUser(userId: string, roleIds: string[]): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${userId}/roles`, roleIds)
      .pipe(catchError(this.handleError));
  }

  private handleError(error: any) {
    console.error('User service error:', error);
    return throwError(() => error);
  }
}
