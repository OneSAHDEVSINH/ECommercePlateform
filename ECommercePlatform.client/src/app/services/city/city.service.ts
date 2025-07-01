import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { City } from '../../models/city.model';
import { environment } from '../../../environments/environment';
import { PagedRequest, PagedResponse } from '../../models/pagination.model';

@Injectable({
  providedIn: 'root'
})
export class CityService {
  private apiUrl = `${environment.apiUrl}/city`;
  //private apiUrl = '/city';
  private headers = new HttpHeaders({
    'Content-Type': 'application/json'
  });
  constructor(private http: HttpClient) { }

  getCities(): Observable<City[]> {
    return this.http.get<City[]>(this.apiUrl);
  }

  // Add to city.service.ts
  getPagedCities(request: PagedRequest, stateId?: string, countryId?: string, activeOnly?: boolean): Observable<PagedResponse<City>> {
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

    if (stateId) {
      params = params.set('stateId', stateId);
    }

    if (countryId) {
      params = params.set('countryId', countryId);
    }

    if (activeOnly !== undefined) {
      params = params.set('activeOnly', activeOnly);
    }

    return this.http.get<PagedResponse<City>>(`${this.apiUrl}/paged`, { params });
  }

  getCitiesByState(stateId: string): Observable<City[]> {
    return this.http.get<City[]>(`${this.apiUrl}/ByState/${stateId}`);
  }

  getCity(id: string): Observable<City> {
    return this.http.get<City>(`${this.apiUrl}/${id}`);
  }

  createCity(city: City): Observable<City> {
    return this.http.post<City>(this.apiUrl, city);
  }

  updateCity(id: string, city: any): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, { ...city, id });
  }

  deleteCity(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
