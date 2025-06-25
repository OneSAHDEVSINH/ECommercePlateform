import { Injectable, OnDestroy } from '@angular/core';
import { Observable, Subject, interval, of } from 'rxjs';
import { filter, switchMap, takeUntil, tap } from 'rxjs/operators';
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
        filter(() => this.authService.isAuthenticated()),
        switchMap(() => this.refreshPermissions())
      )
      .subscribe();
  }

  refreshPermissions(): Observable<boolean> {
    // Skip refresh for superadmin since they always have all permissions
    if (this.authService.isSuperAdmin()) {
      return of(true);
    }
    this.authorizationService.clearCache();
    return this.http.get<any>(`${this.apiUrl}/user-permissions`).pipe(
      tap(response => {
        this.authService.updatePermissionsData(response.permissions);
        this.authorizationService.clearCache();
      }),
      // Force a delay to ensure all subscribers receive the updates
      tap(() => console.log('Permissions refreshed from server')),
      switchMap(() => of(true))
    );
  }
}
