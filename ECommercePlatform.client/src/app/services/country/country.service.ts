import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { catchError, Observable, tap, throwError } from 'rxjs';
import { Country } from '../../models/country.model';
import { environment } from '../../../environments/environment';
import { PagedRequest, PagedResponse } from '../../models/pagination.model';

@Injectable({
  providedIn: 'root'
})
export class CountryService {
  private apiUrl = `${environment.apiUrl}/country`;
  private headers = new HttpHeaders({
    'Content-Type': 'application/json'
  });
  //private apiUrl = '/country';
  constructor(private http: HttpClient) { }

  getCountries(): Observable<Country[]> {
    return this.http.get<Country[]>(this.apiUrl);
  }

  getPagedCountries(request: PagedRequest): Observable<PagedResponse<Country>> {
    let params = new HttpParams()
      .set('pageNumber', request.pageNumber.toString())
      .set('pageSize', request.pageSize.toString());

    if (request.sortColumn) {
      params = params.set('sortColumn', request.sortColumn);
    }

    if (request.sortDirection) {
      params = params.set('sortDirection', request.sortDirection);
    }

    if (request.searchText) {
      params = params.set('searchText', request.searchText);
    }

    // Format dates in ISO format for ASP.NET Core
    if (request.startDate) {
      // Ensure we send the ISO string format that .NET can parse
      params = params.set('startDate', request.startDate);
    }

    if (request.endDate) {
      params = params.set('endDate', request.endDate);
    }

    return this.http.get<PagedResponse<Country>>(`${this.apiUrl}/paged`, { params });
  }

  getCountry(id: string): Observable<Country> {
    return this.http.get<Country>(`${this.apiUrl}/${encodeURIComponent(id)}`);
  }

  createCountry(country: Country): Observable<Country> {
    return this.http.post<Country>(this.apiUrl, country);
  }

  updateCountry(id: string, country: Country): Observable<Country> {
    return this.http.put<Country>(`${this.apiUrl}/${encodeURIComponent(id)}`, country);
  }

  deleteCountry(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${encodeURIComponent(id)}`);
  }
}
