import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service';
import { State } from './state.service';

export interface City {
  id?: string;
  name: string;
  stateId: string;
  state?: State;
  isActive: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class CityService {
  private apiUrl = `/city`;

  constructor(private http: HttpClient, private authService: AuthService) { }

  getAuthHeaders(): HttpHeaders {
    const token = this.authService.getToken();
    return new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`
    });
  }

  getCities(): Observable<City[]> {
    return this.http.get<City[]>(this.apiUrl, { headers: this.getAuthHeaders() });
  }

  getCity(id: string): Observable<City> {
    return this.http.get<City>(`${this.apiUrl}/${id}`, { headers: this.getAuthHeaders() });
  }

  getCitiesByState(stateId: string): Observable<City[]> {
    return this.http.get<City[]>(`${this.apiUrl}/ByState/${stateId}`, { headers: this.getAuthHeaders() });
  }

  createCity(city: City): Observable<City> {
    return this.http.post<City>(this.apiUrl, city, { headers: this.getAuthHeaders() });
  }

  updateCity(id: string, city: City): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, city, { headers: this.getAuthHeaders() });
  }

  deleteCity(id: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`, { headers: this.getAuthHeaders() });
  }
} 
