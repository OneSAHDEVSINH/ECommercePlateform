import { Component, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MessageService } from '../../services/general/message.service';
import { AuthService } from '../../services/auth/auth.service';
import { trigger, transition, style, animate } from '@angular/animations';
import { Subscription } from 'rxjs';

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
  templateUrl: './access-denied.component.html',
  styleUrls: ['./access-denied.component.scss']
})
export class AccessDeniedComponent implements OnInit, OnDestroy {
  errorMessage: string = 'You do not have permission to access this resource.';
  troubleshootingMessage: string = '';
  returnUrl: string | null = null;
  isLoggedIn: boolean = false;
  referenceId: string = ''; // Store the reference ID
  private userSubscription?: Subscription;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private messageService: MessageService,
    private authService: AuthService,
    private cdr: ChangeDetectorRef
  ) {
    // Generate reference ID once in constructor
    this.referenceId = this.generateReferenceId();
  }

  ngOnInit() {
    // Check if user is logged in
    this.userSubscription = this.authService.currentUser$.subscribe(user => {
      this.isLoggedIn = !!user;
      // Use setTimeout to avoid ExpressionChangedAfterItHasBeenCheckedError
      setTimeout(() => {
        this.cdr.detectChanges();
      });
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

      // Use setTimeout to avoid ExpressionChangedAfterItHasBeenCheckedError
      setTimeout(() => {
        this.cdr.detectChanges();
      });
    });
  }

  ngOnDestroy() {
    if (this.userSubscription) {
      this.userSubscription.unsubscribe();
    }
  }

  private setTroubleshootingMessage(module: string, permission: string): void {
    if (module === 'dashboard') {
      this.troubleshootingMessage = 'All users should have access to the dashboard. Try logging out and logging back in.';
    } else if (module === 'users' || module === 'roles' || module === 'modules') {
      this.troubleshootingMessage = `Administrative privileges required. Contact your system administrator.`;
    } else {
      this.troubleshootingMessage = `Contact your administrator to grant ${permission} permission for ${module}.`;
    }
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/admin/login']);
  }

  goBack(): void {
    window.history.back();
  }

  // Use the stored reference ID instead of generating a new one each time
  generateReferenceId(): string {
    return Date.now().toString(36).toUpperCase();
  }

  // Getter method to return the stored reference ID
  getReferenceId(): string {
    return this.referenceId;
  }
}
