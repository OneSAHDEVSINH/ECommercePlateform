import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { Module, Permission } from '../../models/role.model';
import { PagedRequest, PagedResponse } from '../../models/pagination.model';

@Injectable({
  providedIn: 'root'
})
export class ModuleService {
  private apiUrl = `${environment.apiUrl}/module`;

  constructor(private http: HttpClient) { }

  getModules(): Observable<Module[]> {
    return this.http.get<Module[]>(this.apiUrl)
      .pipe(catchError(this.handleError));
  }

  getPagedModules(request: PagedRequest): Observable<PagedResponse<Module>> {
    return this.http.get<PagedResponse<Module>>(`${this.apiUrl}/paged`, {
      params: { ...request as any }
    }).pipe(catchError(this.handleError));
  }

  getModule(id: string): Observable<Module> {
    return this.http.get<Module>(`${this.apiUrl}/${id}`)
      .pipe(catchError(this.handleError));
  }

  getModuleWithPermissions(id: string): Observable<Module> {
    return this.http.get<Module>(`${this.apiUrl}/${id}/permissions`)
      .pipe(catchError(this.handleError));
  }

  getAllModulesWithPermissions(): Observable<Module[]> {
    return this.http.get<Module[]>(`${this.apiUrl}/with-permissions`)
      .pipe(catchError(this.handleError));
  }

  createModule(module: Module): Observable<Module> {
    return this.http.post<Module>(this.apiUrl, module)
      .pipe(catchError(this.handleError));
  }

  updateModule(id: string, module: Module): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, module)
      .pipe(catchError(this.handleError));
  }

  deleteModule(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`)
      .pipe(catchError(this.handleError));
  }

  // The following methods would be implemented if you have permission endpoints
  // These are placeholders for now
  createPermission(permission: Permission): Observable<Permission> {
    return this.http.post<Permission>(`${environment.apiUrl}/permission`, permission)
      .pipe(catchError(this.handleError));
  }

  updatePermission(id: string, permission: Permission): Observable<void> {
    return this.http.put<void>(`${environment.apiUrl}/permission/${id}`, permission)
      .pipe(catchError(this.handleError));
  }

  deletePermission(id: string): Observable<void> {
    return this.http.delete<void>(`${environment.apiUrl}/permission/${id}`)
      .pipe(catchError(this.handleError));
  }

  private handleError(error: any) {
    console.error('Module service error:', error);
    return throwError(() => error);
  }
}
