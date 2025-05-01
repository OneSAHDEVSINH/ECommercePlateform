import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environment';
import { AuthService } from './auth.service';
import { Country } from './country.service';

export interface State {
  id?: string;
  name: string;
  code: string;
  countryId: string;
  country?: Country;
  isActive: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class StateService {
  private apiUrl = `${environment.apiUrl}/state`;

  constructor(private http: HttpClient, private authService: AuthService) { }

  getAuthHeaders(): HttpHeaders {
    const token = this.authService.getToken();
    return new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`
    });
  }

  getStates(): Observable<State[]> {
    return this.http.get<State[]>(this.apiUrl, { headers: this.getAuthHeaders() });
  }

  getState(id: string): Observable<State> {
    return this.http.get<State>(`${this.apiUrl}/${id}`, { headers: this.getAuthHeaders() });
  }

  getStatesByCountry(countryId: string): Observable<State[]> {
    return this.http.get<State[]>(`${this.apiUrl}/ByCountry/${countryId}`, { headers: this.getAuthHeaders() });
  }

  createState(state: State): Observable<State> {
    return this.http.post<State>(this.apiUrl, state, { headers: this.getAuthHeaders() });
  }

  updateState(id: string, state: State): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, state, { headers: this.getAuthHeaders() });
  }

  deleteState(id: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`, { headers: this.getAuthHeaders() });
  }
} 