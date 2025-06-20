import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MessageService } from '../services/general/message.service';
import { AuthService } from '../services/auth/auth.service';

@Component({
  selector: 'app-access-denied',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="hold-transition {{currentTheme}}">
      <div class="wrapper">
        <!-- Navbar -->
        <nav class="main-header navbar navbar-expand navbar-white navbar-light">
          <ul class="navbar-nav ml-auto">
            <li class="nav-item dropdown">
              <a class="nav-link" role="button">
                <i class="fas fa-user-circle"></i> {{ userName }}
              </a>
            </li>
          </ul>
        </nav>
        
        <!-- Sidebar placeholder -->
        <aside class="main-sidebar sidebar-dark-primary elevation-4">
          <a href="#" class="brand-link">
            <i class="fas fa-shopping-cart brand-image img-circle elevation-3" style="opacity: .8"></i>
            <span class="brand-text font-weight-light">E-Commerce Admin</span>
          </a>
          <div class="sidebar">
            <nav class="mt-2">
              <ul class="nav nav-pills nav-sidebar flex-column">
                <li class="nav-item">
                  <a routerLink="/admin/dashboard" class="nav-link">
                    <i class="nav-icon fas fa-tachometer-alt"></i>
                    <p>Dashboard</p>
                  </a>
                </li>
              </ul>
            </nav>
          </div>
        </aside>
        
        <!-- Content -->
        <div class="content-wrapper">
          <div class="content-header">
            <div class="container-fluid">
              <div class="row mb-2">
                <div class="col-sm-6">
                  <h1 class="m-0">Access Denied</h1>
                </div>
                <div class="col-sm-6">
                  <ol class="breadcrumb float-sm-right">
                    <li class="breadcrumb-item"><a routerLink="/admin/dashboard">Dashboard</a></li>
                    <li class="breadcrumb-item active">Access Denied</li>
                  </ol>
                </div>
              </div>
            </div>
          </div>

          <section class="content">
            <div class="error-page">
              <h2 class="headline text-warning">403</h2>
              <div class="error-content">
                <h3><i class="fas fa-exclamation-triangle text-warning"></i> Access Denied!</h3>
                <p>{{ errorMessage }}</p>
                
                <div class="mt-4">
                  <a routerLink="/admin/dashboard" class="btn btn-primary mr-2">
                    <i class="fas fa-home"></i> Return to Dashboard
                  </a>
                  <a *ngIf="returnUrl" [routerLink]="[returnUrl]" class="btn btn-outline-secondary">
                    <i class="fas fa-undo"></i> Try Again
                  </a>
                </div>
              </div>
            </div>
          </section>
        </div>

        <footer class="main-footer">
          <div class="float-right d-none d-sm-inline">
            Version 1.0
          </div>
          <strong>Copyright &copy; 2023-2024 <a href="#">E-Commerce Platform</a>.</strong> All rights reserved.
        </footer>
      </div>
    </div>
  `,
  styles: [`
    .error-page {
      margin: 20px auto;
      width: 600px;
      max-width: 100%;
      padding: 20px;
      color: #444;
    }
    .error-page > .headline {
      float: left;
      font-size: 100px;
      font-weight: 300;
      margin-right: 20px;
      color: #f39c12;
    }
    .error-page > .error-content {
      margin-left: 190px;
      display: block;
    }
    .error-page > .error-content > h3 {
      font-weight: 300;
      font-size: 25px;
    }
  `]
})
export class AccessDeniedComponent implements OnInit {
  errorMessage: string = 'You do not have permission to access this resource.';
  returnUrl: string | null = null;
  userName: string = 'Guest';
  currentTheme: string = 'light-mode';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private messageService: MessageService,
    private authService: AuthService
  ) { }

  ngOnInit() {
    // Get user info if available
    this.authService.currentUser$.subscribe(user => {
      if (user) {
        const firstName = user.firstName || user['firstName'] || '';
        const lastName = user.lastName || user['lastName'] || '';
        this.userName = `${firstName} ${lastName}`.trim() || user.email || 'Guest';
      }
    });

    // Get theme preference
    const savedTheme = localStorage.getItem('admin-theme');
    if (savedTheme) {
      this.currentTheme = savedTheme;
    }

    // Process query params
    this.route.queryParams.subscribe(params => {
      if (params['accessDenied']) {
        const module = params['module'] || 'the requested page';
        const permission = params['permission'] || 'required';
        this.errorMessage = `You don't have ${permission} permission for ${module}.`;
        this.returnUrl = params['returnUrl'] || null;

        this.messageService.showMessage({
          type: 'error',
          text: this.errorMessage
        });
      } else if (params['permissionError']) {
        this.errorMessage = params['errorMessage'] || 'An error occurred while checking permissions.';
        this.messageService.showMessage({
          type: 'error',
          text: this.errorMessage
        });
      }
    });
  }
}
