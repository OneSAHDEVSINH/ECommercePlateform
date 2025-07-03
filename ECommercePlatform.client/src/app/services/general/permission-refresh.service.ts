import { Injectable, OnDestroy } from '@angular/core';
import { Observable, Subject, interval, of, BehaviorSubject } from 'rxjs';
import { filter, switchMap, takeUntil, tap, catchError } from 'rxjs/operators';
import { AuthService } from '../auth/auth.service';
import { AuthorizationService } from '../authorization/authorization.service';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class PermissionRefreshService implements OnDestroy {
  private destroy$ = new Subject<void>();
  private refreshInterval = 30000; // Check every 30 seconds
  private apiUrl = `${environment.apiUrl}/authorization`;

  // Add refresh state tracking
  private isRefreshingSubject = new BehaviorSubject<boolean>(false);
  public isRefreshing$ = this.isRefreshingSubject.asObservable();

  constructor(
    private http: HttpClient,
    private authService: AuthService,
    private authorizationService: AuthorizationService
  ) {
    // Start periodic checking for permission changes
    this.startPeriodicCheck();

    // Listen for window focus events to refresh permissions
    window.addEventListener('focus', this.handleWindowFocus.bind(this));
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
    window.removeEventListener('focus', this.handleWindowFocus.bind(this));
  }

  private handleWindowFocus(): void {
    if (this.authService.isAuthenticated()) {
      this.refreshPermissions().subscribe();
    }
  }

  private startPeriodicCheck(): void {
    interval(this.refreshInterval)
      .pipe(
        takeUntil(this.destroy$),
        filter(() => this.authService.isAuthenticated() && !this.isRefreshingSubject.value),
        switchMap(() => this.refreshPermissions())
      )
      .subscribe();
  }

  refreshPermissions(): Observable<boolean> {
    // Skip refresh for superadmin since they always have all permissions
    if (this.authService.isSuperAdmin()) {
      return of(true);
    }

    this.isRefreshingSubject.next(true);

    return this.http.get<any>(`${this.apiUrl}/user-permissions`).pipe(
      tap(response => {
        // Update permissions in auth service
        this.authService.updatePermissionsData(response.permissions);

        // Clear and refresh authorization cache
        this.authorizationService.clearCache();
        this.authorizationService.refreshPermissions();

        console.log('Permissions refreshed from server');
      }),
      switchMap(() => of(true)),
      catchError(error => {
        console.error('Failed to refresh permissions:', error);
        return of(false);
      }),
      tap(() => {
        this.isRefreshingSubject.next(false);
      })
    );
  }

  // Force immediate refresh
  forceRefresh(): Observable<boolean> {
    return this.refreshPermissions();
  }
}
