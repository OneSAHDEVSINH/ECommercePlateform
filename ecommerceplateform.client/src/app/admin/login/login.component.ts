import { Component, OnInit, OnDestroy, HostListener } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';
import { ThemeService, ThemeType } from '../../services/theme.service';
import {
  trigger,
  transition,
  style,
  animate,
  state,
  keyframes,
  query,
  stagger,
  animateChild
} from '@angular/animations';
import { MessageService, Message } from '../../services/message.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  animations: [
    trigger('fadeIn', [
      state('void', style({ opacity: 0, transform: 'translateY(20px)' })),
      transition(':enter', [
        animate('0.6s {{delay}}ms cubic-bezier(0.35, 0, 0.25, 1)',
          style({ opacity: 1, transform: 'translateY(0)' }))
      ], { params: { delay: 0 } })
    ]),
    trigger('formControls', [
      transition(':enter', [
        style({ opacity: 0, transform: 'translateX(-20px)' }),
        animate('0.5s {{delay}}ms cubic-bezier(0.35, 0, 0.25, 1)',
          style({ opacity: 1, transform: 'translateX(0)' }))
      ], { params: { delay: 100 } })
    ]),
    trigger('buttonPulse', [
      state('initial', style({ transform: 'scale(1)' })),
      state('pulse', style({ transform: 'scale(1.03)' })),
      transition('initial <=> pulse', animate('0.5s cubic-bezier(0.35, 0, 0.25, 1)'))
    ]),
    trigger('slideInFromTop', [
      transition(':enter', [
        style({ opacity: 0, transform: 'translateY(-20px)' }),
        animate('0.6s {{delay}}ms cubic-bezier(0.35, 0, 0.25, 1)',
          style({ opacity: 1, transform: 'translateY(0)' }))
      ], { params: { delay: 0 } })
    ]),
    trigger('pulseAnimation', [
      state('active', style({ transform: 'scale(1)' })),
      transition('* => active', [
        animate('2s ease-in-out', keyframes([
          style({ transform: 'scale(1)', offset: 0 }),
          style({ transform: 'scale(1.05)', offset: 0.5 }),
          style({ transform: 'scale(1)', offset: 1 })
        ]))
      ]),
    ]),
    trigger('rotateAnimation', [
      state('classic', style({ transform: 'rotate(0deg)' })),
      state('modern', style({ transform: 'rotate(360deg)' })),
      transition('classic <=> modern', [
        animate('0.5s cubic-bezier(0.35, 0, 0.25, 1)')
      ])
    ])
  ]
})
export class LoginComponent implements OnInit, OnDestroy {
  loginForm!: FormGroup;
  errorMessage: string | null = null;
  loading: boolean = false;
  returnUrl: string = '/admin/dashboard';
  currentTheme: ThemeType = 'classic';
  buttonState: string = 'initial';
  showPassword: boolean = false;
  particles: number[] = Array(15).fill(0).map((_, i) => i);
  geometricShapes: number[] = Array(8).fill(0).map((_, i) => i);
  message: Message | null = null;
  emailFocused: boolean = false;
  passwordFocused: boolean = false;
  private messageSubscription!: Subscription;
  private buttonAnimInterval: any;
  private themeSubscription!: Subscription;

  constructor(
    private formBuilder: FormBuilder,
    private authService: AuthService,
    private messageService: MessageService,
    private themeService: ThemeService,
    private router: Router,
    private route: ActivatedRoute
  ) { }

  ngOnInit(): void {

    // Subscribe to message changes
    this.messageSubscription = this.messageService.currentMessage.subscribe(message => {
      this.message = message;
    });

    // If there's a message, also update errorMessage for any UI elements that use it directly
    //if (this.message && this.message.type === 'error') {
    //  this.errorMessage = this.message.text;
    //} else if (!this.message) {
    //  this.errorMessage = '';
    //}
  

    // Initialize the form
    this.loginForm = this.formBuilder.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });

    // Get return URL from route parameters or default to '/admin/dashboard'
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/admin/dashboard';

    // If already logged in, redirect to dashboard
    if (this.authService.isAuthenticated() && this.authService.isAdmin()) {
      this.router.navigate([this.returnUrl]);
    }

    // Subscribe to theme changes
    this.themeService.theme$.subscribe(theme => {
      this.currentTheme = theme;
    });

    // Animate login button on interval
    this.buttonAnimInterval = setInterval(() => {
      this.buttonState = this.buttonState === 'initial' ? 'pulse' : 'initial';
    }, 500);
  }

  ngOnDestroy(): void {
    // Clean up subscriptions
    if (this.messageSubscription) {
      this.messageSubscription.unsubscribe();
    }

    // Clear any intervals
    if (this.buttonAnimInterval) {
      clearInterval(this.buttonAnimInterval);
    }
  }

  // Convenience getter for easy access to form fields
  get f() { return this.loginForm.controls; }

  onSubmit(): void {
    // Stop here if form is invalid
    if (this.loginForm.invalid) {
      return;
    }

    this.loading = true;
    this.errorMessage = null;

    this.authService.login({
      email: this.f['email'].value,
      password: this.f['password'].value
    }).subscribe({
      next: (response) => {
        // Check if the user is an admin before redirecting
        if (response.user.role === 'Admin') {
          this.router.navigate([this.returnUrl]);
        } else {
          // Stop loading and show appropriate message for non-admin users
          this.loading = false;
          this.messageService.showMessage({
            type: 'error',
            text: 'Access denied. This portal is for administrators only.'
          }, 2500);
          this.errorMessage = 'Access denied. This portal is for administrators only.';
          setTimeout(() => {
            this.errorMessage = null;
          }, 2500);
          // Log out the user since they don't have admin access
          this.authService.logout();
        }
      },
      error: (error) => {
        const errorText = error.error?.message || 'An error occurred during login';

        this.errorMessage = errorText; // Keep this if you need the errorMessage property elsewhere

        // Only use messageService to display the error
        this.messageService.showMessage({
          type: 'error',
          text: errorText
        }, 2500); // Show error for longer - 2.5 seconds
        
        this.errorMessage = error.error?.message || 'An error occurred during login';

        setTimeout(() => {
          this.errorMessage = null;
        }, 2500);

        this.loading = false;
      }
    });
  }

  toggleTheme(): void {
    this.themeService.toggleTheme();
  }

  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }
}
