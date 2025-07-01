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

  getPagedModules(request: PagedRequest, activeOnly?: boolean): Observable<PagedResponse<Module>> {
    return this.http.get<PagedResponse<Module>>(`${this.apiUrl}/paged`, {
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

  getModule(id: string): Observable<Module> {
    return this.http.get<Module>(`${this.apiUrl}/${id}`)
      .pipe(catchError(this.handleError));
  }

  getModuleByRoute(route: string): Observable<Module> {
    return this.http.get<Module>(`${this.apiUrl}/by-route/${route}`)
      .pipe(catchError(this.handleError));
  }

  createModule(module: Module): Observable<Module> {
    return this.http.post<Module>(this.apiUrl, module)
      .pipe(catchError(this.handleError));
  }

  updateModule(id: string, module: any): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, { ...module, id })
      .pipe(catchError(this.handleError));
  }

  deleteModule(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`)
      .pipe(catchError(this.handleError));
  }

  private handleError(error: any) {
    console.error('Module service error:', error);
    return throwError(() => error);
  }
}
