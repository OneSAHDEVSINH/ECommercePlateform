import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { State } from '../../models/state.model';
import { environment } from '../../../environments/environment';
import { PagedRequest, PagedResponse } from '../../models/pagination.model';

@Injectable({
  providedIn: 'root'
})
export class StateService {
  private apiUrl = `${environment.apiUrl}/state`;
  //private apiUrl = '/state';
  private headers = new HttpHeaders({
    'Content-Type': 'application/json'
  });
  constructor(private http: HttpClient) { }

  getStates(): Observable<State[]> {
    return this.http.get<State[]>(this.apiUrl);
  }

  // Add pagination method
  getPagedStates(request: PagedRequest, countryId?: string, activeOnly?: boolean): Observable<PagedResponse<State>> {
    let params = new HttpParams()
      .set('pageNumber', request.pageNumber.toString())
      .set('pageSize', request.pageSize.toString())
      .set('sortColumn', request.sortColumn || '')
      .set('sortDirection', request.sortDirection || '');

    if (request.searchText) {
      params = params.set('searchText', request.searchText);
    }

    if (request.sortColumn) {
      params = params.set('sortColumn', request.sortColumn);
      params = params.set('sortDirection', request.sortDirection || 'asc');
    }

    // Add date range parameters if they exist
    if (request.startDate) {
      params = params.set('startDate', request.startDate);
    }

    if (request.endDate) {
      params = params.set('endDate', request.endDate);
    }

    if (countryId) {
      params = params.set('countryId', countryId);
    }

    if (activeOnly !== undefined) {
      params = params.set('activeOnly', activeOnly);
    }

    // Use the params object directly in the http options
    return this.http.get<PagedResponse<State>>(`${this.apiUrl}/paged`, { params });
  }

  getStatesByCountry(countryId: string): Observable<State[]> {
    return this.http.get<State[]>(`${this.apiUrl}/ByCountry/${countryId}`);
  }

  getState(id: string): Observable<State> {
    return this.http.get<State>(`${this.apiUrl}/${id}`);
  }

  createState(state: State): Observable<State> {
    return this.http.post<State>(this.apiUrl, state);
  }

  updateState(id: string, state: any): Observable<State> {
    return this.http.put<State>(`${this.apiUrl}/${id}`, { ...state, id });
  }

  deleteState(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
