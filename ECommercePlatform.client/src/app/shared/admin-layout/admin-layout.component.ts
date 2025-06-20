import { Component, OnInit, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router, ActivatedRoute } from '@angular/router';
import { AuthService } from '../../services/auth/auth.service';
import { PermissionDirective } from '../../directives/permission.directive';
import { PermissionType } from '../../models/role.model';

@Component({
  selector: 'app-admin-layout',
  templateUrl: './admin-layout.component.html',
  styleUrls: ['./admin-layout.component.scss'],
  standalone: true,
  imports: [CommonModule, RouterModule, PermissionDirective]
})
export class AdminLayoutComponent implements OnInit {
  userName: string = 'Admin';
  currentTheme: string = 'light-mode';
  isUserDropdownOpen: boolean = false;
  isModulesMenuOpen: boolean = false;
  PermissionType = PermissionType;
  accessDeniedMessage: string | null = null;

  constructor(
    public authService: AuthService,
    private router: Router,
    private route: ActivatedRoute
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

    // Handle access denied messages from query params
    this.route.queryParams.subscribe(params => {
      if (params['accessDenied']) {
        const module = params['module'] || 'the requested page';
        const permission = params['permission'] || 'required';
        this.accessDeniedMessage = `Access denied: You don't have ${permission} permission for ${module}.`;

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
