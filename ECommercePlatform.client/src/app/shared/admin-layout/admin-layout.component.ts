import { Component, OnInit, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router, ActivatedRoute, NavigationEnd } from '@angular/router';
import { Subscription, interval } from 'rxjs';
import { filter, switchMap } from 'rxjs/operators';
import { AuthService } from '../../services/auth/auth.service';
import { PermissionDirective } from '../../directives/permission.directive';
import { PermissionType } from '../../models/role.model';
import { AuthorizationService } from '../../services/authorization/authorization.service';
import { PermissionNotificationService, PermissionError } from '../../services/general/permission-notification.service';
import { PermissionRefreshService } from '../../services/general/permission-refresh.service';


@Component({
  selector: 'app-admin-layout',
  templateUrl: './admin-layout.component.html',
  styleUrls: ['./admin-layout.component.scss'],
  standalone: true,
  imports: [CommonModule, RouterModule]
})
export class AdminLayoutComponent implements OnInit {
  userName: string = 'Admin';
  currentTheme: string = 'light-mode';
  isUserDropdownOpen: boolean = false;
  isModulesMenuOpen: boolean = false;
  PermissionType = PermissionType;
  accessDeniedMessage: string | null = null;
  permissionError: PermissionError | null = null;
  isRefreshing = false;
  private permissionCheckInterval: Subscription | null = null;
  private currentModuleRoute: string = '';

  constructor(
    public authService: AuthService,
    public authorizationService: AuthorizationService,
    private router: Router,
    private route: ActivatedRoute,
    private permissionNotificationService: PermissionNotificationService,
    private permissionRefreshService: PermissionRefreshService
  ) { }

  ngOnInit(): void {
    // Get user info
    this.authService.currentUser$.subscribe(user => {
      if (user) {
        const firstName = user.firstName || user['firstName'] || '';
        const lastName = user.lastName || user['lastName'] || '';
        this.userName = `${firstName} ${lastName}`.trim() || user.email || 'Admin';
      }
    });

    // Initialize theme
    const savedTheme = localStorage.getItem('admin-theme');
    if (savedTheme) {
      this.currentTheme = savedTheme;
    }

    // Subscribe to permission errors
    this.permissionNotificationService.error$.subscribe(error => {
      this.permissionError = error;
    });

    // Detect current route to know which module we're on
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe((event: NavigationEnd) => {
      const urlParts = event.url.split('/');
      if (urlParts.length > 2) {
        this.currentModuleRoute = urlParts[2];
        this.startPermissionCheck();
      }
    });

    // Handle access denied messages from query params
    this.route.queryParams.subscribe(params => {
      if (params['accessDenied']) {
        const module = params['module'] || 'the requested page';
        const permission = params['permission'] || 'required';
        //this.accessDeniedMessage = `Access denied: You don't have ${permission} permission for ${module}.`;

        // Clear the message after 5 seconds
        setTimeout(() => {
          this.accessDeniedMessage = null;

          // Clean up URL query params
          this.router.navigate([], {
            relativeTo: this.route,
            queryParams: {},
            replaceUrl: true
          });
        }, 5000);
      }
    });
    // Check if we're on a modules page to auto-open the menu
    this.isModulesMenuOpen = this.router.url.includes('/admin/modules');
  }

  ngOnDestroy() {
    this.stopPermissionCheck();
  }

  toggleTheme(): void {
    this.currentTheme = this.currentTheme === 'light-mode' ? 'dark-mode' : 'light-mode';
    localStorage.setItem('admin-theme', this.currentTheme);
  }

  toggleUserDropdown(event: Event): void {
    event.preventDefault();
    event.stopPropagation();
    this.isUserDropdownOpen = !this.isUserDropdownOpen;
  }

  // Add method to toggle modules dropdown menu
  toggleModulesMenu(): void {
    this.isModulesMenuOpen = !this.isModulesMenuOpen;
  }

  refreshPermissions(): void {
    this.isRefreshing = true;
    this.permissionRefreshService.refreshPermissions().subscribe({
      next: () => {
        this.isRefreshing = false;

        // Check if we still have permission for current route after refresh
        if (this.currentModuleRoute && this.currentModuleRoute !== 'dashboard' &&
          this.currentModuleRoute !== 'access-denied') {
          this.authorizationService.checkPermission(this.currentModuleRoute, PermissionType.View)
            .subscribe(hasPermission => {
              if (!hasPermission && !this.authorizationService.isAdmin()) {
                this.router.navigate(['/admin/access-denied'], {
                  queryParams: {
                    accessDenied: 'true',
                    module: this.currentModuleRoute,
                    permission: 'View',
                    returnUrl: this.router.url
                  }
                });
              }
            });
        }
      },
      error: (err) => {
        console.error('Failed to refresh permissions', err);
        this.isRefreshing = false;
      }
    });
  }

  private startPermissionCheck() {
    // Stop any existing checks
    this.stopPermissionCheck();

    // Don't check for dashboard or exempt routes
    if (!this.currentModuleRoute || this.currentModuleRoute === 'dashboard' ||
      this.currentModuleRoute === 'access-denied' || this.currentModuleRoute === 'login') {
      return;
    }

    // Check every 15 seconds if we still have view permission for the current module
    this.permissionCheckInterval = interval(15000).pipe(
      switchMap(() => this.permissionRefreshService.refreshPermissions()),
      switchMap(() => this.authorizationService.checkPermission(this.currentModuleRoute, PermissionType.View))
    ).subscribe(hasPermission => {
      if (!hasPermission && !this.authorizationService.isAdmin()) {
        console.warn(`Permission lost for module: ${this.currentModuleRoute}`);
        this.router.navigate(['/admin/access-denied'], {
          queryParams: {
            accessDenied: 'true',
            module: this.currentModuleRoute,
            permission: 'View',
            returnUrl: this.router.url
          }
        });
      }
    });
  }

  private stopPermissionCheck() {
    if (this.permissionCheckInterval) {
      this.permissionCheckInterval.unsubscribe();
      this.permissionCheckInterval = null;
    }
  }

  dismissPermissionError(): void {
    this.permissionNotificationService.clearError();
  }

  @HostListener('document:click')
  closeDropdown(): void {
    this.isUserDropdownOpen = false;
    // Don't auto-close the modules menu on document click as that would be disruptive
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/admin/login']);
  }
}
