import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth/auth.service';
import { jwtDecode } from 'jwt-decode';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule]
})
export class LoginComponent implements OnInit, OnDestroy {
  loginForm!: FormGroup;
  errorMessage: string = '';
  loading: boolean = false;
  showPassword: boolean = false;
  returnUrl: string = '/admin/dashboard';

  constructor(
    private formBuilder: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute
  ) { }

  ngOnInit(): void {
    // Add AdminLTE classes
    document.body.classList.add('hold-transition', 'login-page');

    // Initialize form
    this.loginForm = this.formBuilder.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });

    // Get return URL with validation
    this.returnUrl = this.getValidReturnUrl();

    // Check if already logged in
    if (this.authService.isAuthenticated() && this.authService.hasAnyRole()) {
      this.router.navigate([this.returnUrl]);
    }
    this.route.queryParams.subscribe(params => {
      if (params['accessDenied']) {
        this.loading = false;
        const module = params['module'] || 'the requested page';
        const permission = params['permission'] || 'required';
        //this.errorMessage = `Access denied: You don't have ${permission} permission for ${module}.`;

        // Clean up URL query params after displaying message
        this.router.navigate([], {
          relativeTo: this.route,
          queryParams: {},
          replaceUrl: true
        });
      }
      setTimeout(() => {
        this.errorMessage = '';
      }, 2500);
      this.loading = false;
    });
  }

  private getValidReturnUrl(): string {
    const defaultUrl = '/admin/dashboard';
    const queryReturnUrl = this.route.snapshot.queryParams['returnUrl'];

    if (!queryReturnUrl) {
      return defaultUrl;
    }

    try {
      // Try to decode the URL to check if it's valid
      const decodedUrl = decodeURIComponent(queryReturnUrl);

      // Additional validation - ensure it starts with /
      if (!decodedUrl.startsWith('/')) {
        console.warn('Invalid return URL - must start with /:', decodedUrl);
        return defaultUrl;
      }

      // Prevent open redirect vulnerabilities
      if (decodedUrl.includes('//') || decodedUrl.includes('\\')) {
        console.warn('Invalid return URL - possible redirect attack:', decodedUrl);
        return defaultUrl;
      }

      return decodedUrl;
    } catch (e) {
      console.error('Malformed return URL:', queryReturnUrl);
      // If URL is malformed, return to default
      return defaultUrl;
    }
  }

  ngOnDestroy(): void {
    // Remove AdminLTE classes
    document.body.classList.remove('hold-transition', 'login-page');
  }

  get f() { return this.loginForm.controls; }

  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }

  onSubmit(): void {
    if (this.loginForm.invalid) {
      return;
    }

    this.loading = true;
    this.errorMessage = '';

    const loginData = {
      email: this.f['email'].value,
      password: this.f['password'].value
    };

    console.log('Login attempt with:', loginData);

    this.authService.login(loginData).subscribe({
      next: (response) => {
        console.log('Login successful:', response);

        // Check if user has any role
        if (this.authService.hasAnyRole() || this.authService.isSuperAdmin()) {
          // Navigate to return URL or dashboard
          this.router.navigate([this.returnUrl]).catch(() => {
            this.router.navigate(['/admin/dashboard']);
          });
        } else {
          this.loading = false;
          this.errorMessage = 'Access denied. You don\'t have any assigned roles. Please contact the administrator.';
          this.authService.logout();
        }
      },
      error: (error) => {
        this.loading = false;
        console.error('Login error:', error);
        console.error('Error response:', error.error);

        // Handle specific error messages
        if (error.status === 401) {
          this.errorMessage = 'Invalid email or password';
        } else if (error.error?.message) {
          this.errorMessage = error.error.message;
        } else {
          this.errorMessage = 'Login failed. Please try again.';
        }
        setTimeout(() => {
          this.errorMessage = '';
        }, 2500);
      }
    });
  }
}
