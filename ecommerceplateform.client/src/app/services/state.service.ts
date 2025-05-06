import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { State } from '../models/state.model';
import { environment } from '../../environments/environment';

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

  getStatesByCountry(countryId: string): Observable<State[]> {
    return this.http.get<State[]>(`${this.apiUrl}/ByCountry/${countryId}`);
  }

  getState(id: string): Observable<State> {
    return this.http.get<State>(`${this.apiUrl}/${id}`);
  }

  createState(state: State): Observable<State> {
    return this.http.post<State>(this.apiUrl, state);
  }

  updateState(id: string, state: State): Observable<State> {
    return this.http.put<State>(`${this.apiUrl}/${id}`, state);
  }

  deleteState(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
