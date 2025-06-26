import { Component, OnInit, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router, ActivatedRoute, NavigationEnd } from '@angular/router';
import { Subscription, interval } from 'rxjs';
import { filter, switchMap } from 'rxjs/operators';
import { AuthService } from '../../services/auth/auth.service';
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
  isQuickActionsOpen: boolean = false;
  isAccessManagementOpen: boolean = false;
  isSidebarCollapsed: boolean = false;
  screenWidth: number = window.innerWidth;
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

    // Check for saved sidebar state
    const savedSidebarState = localStorage.getItem('admin-sidebar-collapsed');
    this.isSidebarCollapsed = savedSidebarState === 'true';

    // Auto-collapse sidebar on mobile devices
    if (this.screenWidth < 992) {
      this.isSidebarCollapsed = true;
    }

    // Subscribe to permission errors
    this.permissionNotificationService.error$.subscribe(error => {
      this.permissionError = error;
    });

    // Detect current route to know which module user is on
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe((event: NavigationEnd) => {
      const urlParts = event.url.split('/');
      if (urlParts.length > 2) {
        this.currentModuleRoute = urlParts[2];
        this.startPermissionCheck();

        // Auto-collapse sidebar on navigation in mobile view
        if (this.screenWidth < 992) {
          this.isSidebarCollapsed = true;
        }

        // Check if the active route belongs to any dropdown and open it
        this.updateActiveDropdown(this.currentModuleRoute);
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

    // Initialize active dropdown based on current route
    this.updateActiveDropdown(this.currentModuleRoute);
  }

  updateActiveDropdown(route: string): void {
    // Quick Actions routes
    if (route === 'countries' || route === 'states' || route === 'cities') {
      this.isQuickActionsOpen = true;
      this.isAccessManagementOpen = false;
    }
    // Access Management routes
    else if (route === 'users' || route === 'roles' || route === 'modules') {
      this.isAccessManagementOpen = true;
      this.isQuickActionsOpen = false;
    }
  }

  @HostListener('window:resize', ['$event'])
  onResize(event: any) {
    this.screenWidth = window.innerWidth;
    // Auto-collapse sidebar on small screens
    if (this.screenWidth < 992 && !this.isSidebarCollapsed) {
      this.isSidebarCollapsed = true;
    }
  }

  toggleSidebar(event?: Event): void {
    if (event) {
      event.preventDefault();
    }
    this.isSidebarCollapsed = !this.isSidebarCollapsed;
    localStorage.setItem('admin-sidebar-collapsed', this.isSidebarCollapsed.toString());
  }

  toggleQuickActions(event?: Event): void {
    if (event) {
      event.preventDefault();
    }
    this.isQuickActionsOpen = !this.isQuickActionsOpen;
    // Close other dropdown when opening this one
    if (this.isQuickActionsOpen) {
      this.isAccessManagementOpen = false;
    }
  }

  toggleAccessManagement(event?: Event): void {
    if (event) {
      event.preventDefault();
    }
    this.isAccessManagementOpen = !this.isAccessManagementOpen;
    // Close other dropdown when opening this one
    if (this.isAccessManagementOpen) {
      this.isQuickActionsOpen = false;
    }
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

  refreshPermissions(): void {
    this.isRefreshing = true;
    this.permissionRefreshService.refreshPermissions().subscribe({
      next: () => {
        this.isRefreshing = false;

        // Check if still have permission for current route after refresh
        if (this.currentModuleRoute && this.currentModuleRoute !== 'dashboard' &&
          this.currentModuleRoute !== 'access-denied') {
          this.authorizationService.checkPermission(this.currentModuleRoute, PermissionType.View)
            .subscribe(hasPermission => {
              if (!hasPermission && !this.authorizationService.isAdmin()) {
                // Don't redirect to access-denied if already there
                if (!this.router.url.includes('/admin/access-denied')) {
                  this.router.navigate(['/admin/access-denied'], {
                    queryParams: {
                      accessDenied: 'true',
                      module: this.currentModuleRoute,
                      permission: 'View',
                      returnUrl: this.router.url
                    }
                  });
                }
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

    // Check every 15 seconds if still have view permission for the current module
    this.permissionCheckInterval = interval(1000).pipe(
      switchMap(() => this.permissionRefreshService.refreshPermissions()),
      switchMap(() => this.authorizationService.checkPermission(this.currentModuleRoute, PermissionType.View))
    ).subscribe(hasPermission => {
      if (!hasPermission && !this.authorizationService.isAdmin()) {
        console.warn(`Permission lost for module: ${this.currentModuleRoute}`);

        // Don't redirect to access-denied if already there to prevent recursive URLs
        if (!this.router.url.includes('/admin/access-denied')) {
          this.router.navigate(['/admin/access-denied'], {
            queryParams: {
              accessDenied: 'true',
              module: this.currentModuleRoute,
              permission: 'View',
              returnUrl: this.router.url
            }
          });
        }
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
    // Don't auto-close the sidebar dropdowns on document click as that would be disruptive
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/admin/login']);
  }

  // Helper method to check if a route is active
  isRouteActive(route: string): boolean {
    return this.router.url.includes(`/admin/${route}`);
  }
}
