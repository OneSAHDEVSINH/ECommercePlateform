import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';
import { ThemeService, ThemeType } from '../../services/theme.service';
import { trigger, transition, style, animate, state } from '@angular/animations';
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
        animate('0.5s ease-out', style({ opacity: 1, transform: 'translateY(0)' }))
      ])
    ]),
    trigger('formControls', [
      transition(':enter', [
        style({ opacity: 0, transform: 'translateX(-20px)' }),
        animate('0.5s 0.3s ease-out', style({ opacity: 1, transform: 'translateX(0)' }))
      ])
    ]),
    trigger('buttonPulse', [
      state('initial', style({ transform: 'scale(1)' })),
      state('pulse', style({ transform: 'scale(1.05)' })),
      transition('initial <=> pulse', animate('0.3s ease-in-out'))
    ])
  ]
})
export class LoginComponent implements OnInit, OnDestroy {
  loginForm!: FormGroup;
  //errorMessage: string = '';
  errorMessage: Message | null = null;
  loading: boolean = false;
  returnUrl: string = '/admin/dashboard';
  currentTheme: ThemeType = 'classic';
  buttonState: string = 'initial';
  showPassword: boolean = false;
  particles: number[] = Array(10).fill(0).map((_, i) => i);
  message: Message | null = null;
  private messageSubscription!: Subscription;
  private buttonAnimInterval: any;

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
    }, 3000);
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
    //this.errorMessage = '';

    this.authService.login({
      email: this.f['email'].value,
      password: this.f['password'].value
    }).subscribe({
      next: () => {
        this.router.navigate([this.returnUrl]);
      },
      error: (error) => {
        const errorText = error.error?.message || 'An error occurred during login';

        this.errorMessage = errorText; // Keep this if you need the errorMessage property elsewhere

        // Only use messageService to display the error
        this.messageService.showMessage({
          type: 'error',
          text: errorText
        }, 2500); // Show error for longer - 5 seconds
        
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
