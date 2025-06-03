import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, Subject, BehaviorSubject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { PagedRequest, PagedResponse } from '../../models/pagination.model';

@Injectable({
  providedIn: 'root'
})
export class ListService {
  private searchTerms = new Subject<string>();
  private searchDebounce = this.searchTerms.pipe(
    debounceTime(300),
    distinctUntilChanged()
  );

  constructor(private http: HttpClient) { }

  getSearchObservable(): Observable<string> {
    return this.searchDebounce;
  }

  search(term: string): void {
    this.searchTerms.next(term);
  }

  getPagedData<T>(url: string, request: PagedRequest): Observable<PagedResponse<T>> {
    let params = new HttpParams()
      .set('pageNumber', request.pageNumber.toString())
      .set('pageSize', request.pageSize.toString());

    if (request.searchText) {
      params = params.set('searchText', request.searchText);
    }

    if (request.sortColumn) {
      params = params.set('sortColumn', request.sortColumn);
      params = params.set('sortDirection', request.sortDirection || 'asc');
    }

    if (request.filters) {
      for (const key in request.filters) {
        if (request.filters[key] !== undefined && request.filters[key] !== null) {
          params = params.set(key, request.filters[key].toString());
        }
      }
    }

    return this.http.get<PagedResponse<T>>(url, { params });
  }
}
