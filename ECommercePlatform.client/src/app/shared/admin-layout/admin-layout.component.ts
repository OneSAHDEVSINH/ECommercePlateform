import { Component, OnInit, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { AuthService } from '../../services/auth/auth.service';

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

  constructor(
    public authService: AuthService,
    private router: Router
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

  @HostListener('document:click')
  closeDropdown(): void {
    this.isUserDropdownOpen = false;
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/admin/login']);
  }
}
