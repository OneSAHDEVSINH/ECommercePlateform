import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ThemeService } from '../../services/general/theme.service';
import { AuthService } from '../../services/auth/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-admin-layout',
  templateUrl: './admin-layout.component.html',
  styleUrls: ['./admin-layout.component.scss'],
  standalone: true,
  imports: [CommonModule, RouterModule]
})
export class AdminLayoutComponent implements OnInit {
  userName: string = '';
  currentTheme: string = 'light-mode';

  constructor(
    private themeService: ThemeService,
    private authService: AuthService,
    private router: Router
  ) { }

  ngOnInit(): void {
    // Get user info
    this.authService.currentUser$.subscribe(user => {
      if (user) {
        const firstName = user.firstName || user['firstName'] || '';
        const lastName = user.lastName || user['lastName'] || '';
        this.userName = `${firstName} ${lastName}`.trim() || user.email;
        console.log('User name set to:', this.userName);
      }
    });

    // Subscribe to theme changes
    this.themeService.theme$.subscribe(theme => {
      this.currentTheme = theme === 'modern' ? 'dark-mode' : 'light-mode';
      document.body.className = this.currentTheme;
    });

    // Initialize AdminLTE
    this.setupAdminLTE();
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/admin/login']);
  }

  toggleTheme(): void {
    this.themeService.toggleTheme();
  }

  private setupAdminLTE(): void {
    // This will be called after the view is initialized to activate AdminLTE features
    // We would normally use jQuery here, but we'll use setTimeout to wait for DOM to be ready
    setTimeout(() => {
      // @ts-ignore - AdminLTE is loaded globally
      if (window.$.fn.pushMenu) {
        // @ts-ignore
        $('[data-widget="pushmenu"]').pushMenu();
      }

      // Additional AdminLTE initializations can go here
    }, 100);
  }
}
