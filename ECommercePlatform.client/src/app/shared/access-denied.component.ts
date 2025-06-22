import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MessageService } from '../services/general/message.service';
import { AuthService } from '../services/auth/auth.service';
import { trigger, transition, style, animate } from '@angular/animations';

@Component({
  selector: 'app-access-denied',
  standalone: true,
  imports: [CommonModule, RouterModule],
  animations: [
    trigger('fadeIn', [
      transition(':enter', [
        style({ opacity: 0, transform: 'translateY(-20px)' }),
        animate('400ms ease-out', style({ opacity: 1, transform: 'translateY(0)' }))
      ])
    ]),
    trigger('scaleIn', [
      transition(':enter', [
        style({ opacity: 0, transform: 'scale(0.8)' }),
        animate('500ms ease-out', style({ opacity: 1, transform: 'scale(1)' }))
      ])
    ]),
    trigger('slideIn', [
      transition(':enter', [
        style({ opacity: 0, transform: 'translateX(-30px)' }),
        animate('600ms ease-out', style({ opacity: 1, transform: 'translateX(0)' }))
      ])
    ])
  ],
  template: `
    <div class="access-denied-container">
      <!-- Compact Header -->
      <div class="compact-header">
        <div class="d-flex justify-content-between align-items-center px-3 py-2">
          <h5 class="mb-0 text-dark">
            <i class="fas fa-ban text-warning mr-2"></i>Access Denied
          </h5>
          <div class="breadcrumb mb-0 bg-transparent">
            <a routerLink="/admin/dashboard" class="text-primary">
              <i class="fas fa-home mr-1"></i>Dashboard
            </a>
            <span class="mx-2">/</span>
            <span class="text-muted">Access Denied</span>
          </div>
        </div>
      </div>

      <!-- Main Content Area -->
      <div class="main-content-area">
        <div class="card border-0 shadow-sm h-100" @fadeIn>
          <div class="card-body p-0 h-100">
            <div class="row no-gutters h-100">
              <!-- Left Panel -->
              <div class="col-lg-4 col-md-5 d-flex align-items-center justify-content-center bg-light border-right">
                <div class="text-center p-4" @scaleIn>
                  <div class="icon-wrapper mb-3">
                    <i class="fas fa-lock text-warning"></i>
                  </div>
                  <h1 class="error-code mb-1">403</h1>
                  <p class="text-muted mb-0">FORBIDDEN</p>
                </div>
              </div>

              <!-- Right Panel -->
              <div class="col-lg-8 col-md-7 d-flex align-items-center">
                <div class="w-100 p-4" @slideIn>
                  <!-- Error Message -->
                  <div class="mb-4">
                    <h4 class="text-dark mb-2">
                      <i class="fas fa-exclamation-circle text-warning mr-2"></i>
                      Access Restricted
                    </h4>
                    <div class="alert alert-warning py-2 px-3 mb-3">
                      <i class="fas fa-info-circle mr-2"></i>{{ errorMessage }}
                    </div>
                    <p class="text-muted mb-0" *ngIf="troubleshootingMessage">
                      {{ troubleshootingMessage }}
                    </p>
                  </div>

                  <!-- Quick Info Grid -->
                  <div class="row mb-4">
                    <div class="col-6 mb-2">
                      <div class="d-flex align-items-center">
                        <div class="icon-box bg-primary">
                          <i class="fas fa-question"></i>
                        </div>
                        <div class="ml-3">
                          <small class="text-muted d-block">Reason</small>
                          <span class="font-weight-medium">Insufficient permissions</span>
                        </div>
                      </div>
                    </div>
                    <div class="col-6 mb-2">
                      <div class="d-flex align-items-center">
                        <div class="icon-box bg-success">
                          <i class="fas fa-check"></i>
                        </div>
                        <div class="ml-3">
                          <small class="text-muted d-block">Solution</small>
                          <span class="font-weight-medium">Request access</span>
                        </div>
                      </div>
                    </div>
                    <div class="col-6 mb-2">
                      <div class="d-flex align-items-center">
                        <div class="icon-box bg-info">
                          <i class="fas fa-user"></i>
                        </div>
                        <div class="ml-3">
                          <small class="text-muted d-block">Status</small>
                          <span class="font-weight-medium">{{ isLoggedIn ? 'Logged in' : 'Not logged in' }}</span>
                        </div>
                      </div>
                    </div>
                    <div class="col-6 mb-2">
                      <div class="d-flex align-items-center">
                        <div class="icon-box bg-warning">
                          <i class="fas fa-tag"></i>
                        </div>
                        <div class="ml-3">
                          <small class="text-muted d-block">Reference</small>
                          <span class="font-weight-medium">#{{ generateReferenceId() }}</span>
                        </div>
                      </div>
                    </div>
                  </div>

                  <!-- Action Buttons -->
                  <div class="action-area">
                    <button routerLink="/admin/dashboard" class="btn btn-primary btn-sm px-4">
                      <i class="fas fa-home mr-1"></i>Dashboard
                    </button>
                    <button *ngIf="returnUrl" [routerLink]="[returnUrl]" class="btn btn-info btn-sm px-4">
                      <i class="fas fa-redo mr-1"></i>Try Again
                    </button>
                    <button *ngIf="!returnUrl && isLoggedIn" (click)="goBack()" class="btn btn-secondary btn-sm px-4">
                      <i class="fas fa-arrow-left mr-1"></i>Go Back
                    </button>
                    <button *ngIf="isLoggedIn" (click)="logout()" class="btn btn-outline-danger btn-sm px-4">
                      <i class="fas fa-sign-out-alt mr-1"></i>Re-login
                    </button>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    /* Full viewport container */
    .access-denied-container {
      height: 100vh;
      display: flex;
      flex-direction: column;
      overflow: hidden;
      background-color: #f4f6f9;
    }

    /* Compact header */
    .compact-header {
      background: white;
      border-bottom: 1px solid #dee2e6;
      flex-shrink: 0;
    }

    .breadcrumb {
      padding: 0;
      margin: 0;
      font-size: 0.875rem;
    }

    /* Main content area */
    .main-content-area {
      flex: 1;
      padding: 20px;
      overflow: hidden;
    }

    /* Icon wrapper for lock */
    .icon-wrapper {
      width: 80px;
      height: 80px;
      margin: 0 auto;
      display: flex;
      align-items: center;
      justify-content: center;
      background: rgba(255, 193, 7, 0.1);
      border-radius: 50%;
      position: relative;
    }

    .icon-wrapper i {
      font-size: 2.5rem;
      animation: float 3s ease-in-out infinite;
    }

    @keyframes float {
      0%, 100% { transform: translateY(0); }
      50% { transform: translateY(-5px); }
    }

    /* Error code styling */
    .error-code {
      font-size: 3.5rem;
      font-weight: 700;
      color: #f39c12;
      line-height: 1;
      margin: 0;
    }

    /* Small icon boxes */
    .icon-box {
      width: 35px;
      height: 35px;
      border-radius: 8px;
      display: flex;
      align-items: center;
      justify-content: center;
      color: white;
      font-size: 0.875rem;
    }

    /* Alert compact */
    .alert {
      font-size: 0.95rem;
      border-radius: 4px;
    }

    /* Action area */
    .action-area {
      display: flex;
      gap: 10px;
      flex-wrap: wrap;
      padding-top: 20px;
      border-top: 1px solid #e3e6f0;
    }

    /* Button styling */
    .btn-sm {
      padding: 0.4rem 1.5rem;
      font-size: 0.875rem;
      border-radius: 20px;
    }

    /* Font weight utility */
    .font-weight-medium {
      font-weight: 500;
      font-size: 0.875rem;
    }

    /* Dark mode support */
    .dark-mode .access-denied-container {
      background-color: #1a1a1a;
    }

    .dark-mode .compact-header {
      background: #1f2937;
      border-bottom-color: #374151;
    }

    .dark-mode .main-content-area .card {
      background: #1f2937;
      color: #e5e7eb;
    }

    .dark-mode .bg-light {
      background-color: #374151 !important;
    }

    .dark-mode .border-right {
      border-right-color: #4b5563 !important;
    }

    .dark-mode .text-dark {
      color: #e5e7eb !important;
    }

    .dark-mode .text-muted {
      color: #9ca3af !important;
    }

    .dark-mode .alert-warning {
      background-color: rgba(251, 191, 36, 0.1);
      border-color: #fbbf24;
      color: #fbbf24;
    }

    /* Responsive adjustments */
    @media (max-width: 768px) {
      .main-content-area {
        padding: 10px;
      }

      .compact-header {
        font-size: 0.875rem;
      }

      .col-lg-4 {
        min-height: 200px;
      }

      .error-code {
        font-size: 3rem;
      }

      .icon-wrapper {
        width: 60px;
        height: 60px;
      }

      .icon-wrapper i {
        font-size: 2rem;
      }

      .action-area {
        justify-content: center;
      }

      .btn-sm {
        flex: 1;
        min-width: 120px;
      }
    }

    @media (max-width: 576px) {
      .breadcrumb {
        display: none;
      }

      .p-4 {
        padding: 1.5rem !important;
      }

      .row .col-6 {
        width: 100%;
      }
    }

    /* Ensure no scrollbars */
    * {
      box-sizing: border-box;
    }

    body {
      overflow: hidden;
    }
  `]
})
export class AccessDeniedComponent implements OnInit {
  errorMessage: string = 'You do not have permission to access this resource.';
  troubleshootingMessage: string = '';
  returnUrl: string | null = null;
  isLoggedIn: boolean = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private messageService: MessageService,
    private authService: AuthService
  ) { }

  ngOnInit() {
    // Check if user is logged in
    this.authService.currentUser$.subscribe(user => {
      this.isLoggedIn = !!user;
    });

    // Process query params
    this.route.queryParams.subscribe(params => {
      if (params['accessDenied']) {
        const module = params['module'] || 'the requested page';
        const permission = params['permission'] || 'required';
        this.errorMessage = `You don't have ${permission} permission for ${module}.`;
        this.returnUrl = params['returnUrl'] || null;

        // Set specific troubleshooting message based on module
        this.setTroubleshootingMessage(module, permission);

        this.messageService.showMessage({
          type: 'error',
          text: this.errorMessage
        });
      } else if (params['permissionError']) {
        this.errorMessage = params['errorMessage'] || 'An error occurred while checking permissions.';
        this.troubleshootingMessage = 'There was a technical issue with the permission system. Try refreshing the page or logging out and back in.';

        this.messageService.showMessage({
          type: 'error',
          text: this.errorMessage
        });
      }
    });
  }

  private setTroubleshootingMessage(module: string, permission: string): void {
    if (module === 'dashboard') {
      this.troubleshootingMessage = 'All users should have access to the dashboard. Try logging out and logging back in.';
    } else if (module === 'users' || module === 'roles' || module === 'modules') {
      this.troubleshootingMessage = `Administrative privileges required. Contact your system administrator.`;
    } else {
      this.troubleshootingMessage = `Contact your administrator to grant ${permission} permissionfor ${module}.`;
    }
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/admin/login']);
  }

  goBack(): void {
    window.history.back();
  }

  generateReferenceId(): string {
    return Date.now().toString(36).toUpperCase();
  }
}
