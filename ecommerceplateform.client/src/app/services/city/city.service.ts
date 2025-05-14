import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { City } from '../../models/city.model';
import { environment } from '../../../environments/environment';

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

  getCitiesByState(stateId: string): Observable<City[]> {
    return this.http.get<City[]>(`${this.apiUrl}/ByState/${stateId}`);
  }

  getCity(id: string): Observable<City> {
    return this.http.get<City>(`${this.apiUrl}/${id}`);
  }

  createCity(city: City): Observable<City> {
    return this.http.post<City>(this.apiUrl, city);
  }

  updateCity(id: string, city: City): Observable<City> {
    return this.http.put<City>(`${this.apiUrl}/${id}`, city);
  }

  deleteCity(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
