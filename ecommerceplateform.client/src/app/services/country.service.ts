import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environment';
import { AuthService } from './auth.service';

export interface Country {
  id?: string;
  name: string;
  code: string;
  isActive: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class CountryService {
  private apiUrl = `${environment.apiUrl}/country`;

  constructor(private http: HttpClient, private authService: AuthService) { }

  getAuthHeaders(): HttpHeaders {
    const token = this.authService.getToken();
    return new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`
    });
  }

  getCountries(): Observable<Country[]> {
    return this.http.get<Country[]>(this.apiUrl, { headers: this.getAuthHeaders() });
  }

  getCountry(id: string): Observable<Country> {
    return this.http.get<Country>(`${this.apiUrl}/${id}`, { headers: this.getAuthHeaders() });
  }

  createCountry(country: Country): Observable<Country> {
    return this.http.post<Country>(this.apiUrl, country, { headers: this.getAuthHeaders() });
  }

  updateCountry(id: string, country: Country): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, country, { headers: this.getAuthHeaders() });
  }

  deleteCountry(id: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`, { headers: this.getAuthHeaders() });
  }
} 