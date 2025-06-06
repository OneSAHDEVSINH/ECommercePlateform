import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject, tap, of } from 'rxjs';
import { LoginRequest, LoginResponse, User, UserRole } from '../../models/user.model';
import { environment } from '../../../environments/environment';
import { PagedRequest, PagedResponse } from '../../models/pagination.model';
import { PermissionType } from '../../models/role.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private currentUserSubject = new BehaviorSubject<User | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  private apiUrl = environment.apiUrl;
  //private apiUrl = '/Auth';
  //private apiUrl = 'https://localhost:44362/Auth';

  
  // Development mode flag - REMOVE IN PRODUCTION
  private devMode = false;
  
  constructor(private http: HttpClient) {
    // Check if user is already logged in on initialization
    this.loadCurrentUser();
    
    // Auto-login for development - REMOVE IN PRODUCTION
    if (this.devMode && !this.currentUserSubject.value) {
      this.simulateLogin();
    }
  }

  // Development only - REMOVE IN PRODUCTION
  private simulateLogin(): void {
    const mockUser: User = {
      id: '1',
      firstName: 'Admin',
      lastName: 'User',
      email: 'admin@example.com',
      role: UserRole.Admin,
      isActive: true
    };
    
    localStorage.setItem('token', 'fake-jwt-token');
    localStorage.setItem('currentUser', JSON.stringify(mockUser));
    this.currentUserSubject.next(mockUser);
    console.warn('DEV MODE: Auto-login enabled. Remove before production!');
  }

  // helper method
  getRoleFromString(role: string): UserRole {
    switch (role.toLowerCase()) {
      case 'admin':
        return UserRole.Admin;
      case 'customer':
        return UserRole.Customer;
      case 'vendor':
        return UserRole.Vendor;
      case 'seller':
        return UserRole.Seller;
      default:
        return UserRole.Guest;
    }
  }

  login(credentials: LoginRequest): Observable<LoginResponse> {
    // For development - REMOVE IN PRODUCTION
    if (this.devMode) {
      const mockUser: User = {
        id: '1',
        firstName: 'Admin',
        lastName: 'User',
        email: credentials.email,
        role: UserRole.Admin,
        isActive: true
      };
      
      const response: LoginResponse = {
        token: 'fake-jwt-token',
        user: mockUser
      };
      
      localStorage.setItem('token', response.token);
      localStorage.setItem('currentUser', JSON.stringify(response.user));
      this.currentUserSubject.next(response.user);
      
      return of(response);
    }


    // Real implementation for production
    return this.http.post<LoginResponse>(`${this.apiUrl}/Auth/login`, credentials)
      .pipe(
        tap(response => {
          console.log('Raw API response:', response); // for debugging

          // Store token in local storage
          localStorage.setItem('token', response.token);

          // Create a properly formatted user object
          const user: User = {
            id: response.user.id,
            firstName: response.user.firstName || '',
            lastName: response.user.lastName || '',
            email: response.user.email || '',
            role: this.getRoleFromString(response.user.role) || response.user.role,
            isActive: response.user.isActive || true
          };
          
          // Store user data
          localStorage.setItem('currentUser', JSON.stringify(response.user));
          
          // Update current user subject
          this.currentUserSubject.next(response.user);
        })
      );
  }

  logout(): void {
    // Remove token and user data from local storage
    localStorage.removeItem('token');
    localStorage.removeItem('currentUser');
    
    // Update current user subject
    this.currentUserSubject.next(null);
  }

  private loadCurrentUser(): void {
    const userJson = localStorage.getItem('currentUser');
    if (userJson) {
      const user = JSON.parse(userJson) as User;
      this.currentUserSubject.next(user);
    }
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  isAuthenticated(): boolean {
    return !!this.getToken();
  }

  isAdmin(): boolean {
    const user = this.currentUserSubject.value;
    return !!user && user.role === UserRole.Admin;
  }

  // Check if user has permission
  hasPermission(moduleRoute: string, permissionType: PermissionType): boolean {
    // This is a placeholder. Implement your actual permission check logic
    // For now, returning true to make development easier
    return true;
  }

  // Get users with paging (or use a separate UserService)
  getUsers(pageRequest: PagedRequest): Observable<PagedResponse<User>> {
    return this.http.get<PagedResponse<User>>(`${this.apiUrl}/users`, {
      params: { ...pageRequest as any }
    });
  }

  // Create user
  createUser(userData: User): Observable<User> {
    return this.http.post<User>(`${this.apiUrl}/users`, userData);
  }

  // Update user
  updateUser(userId: string, userData: User): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/users/${userId}`, userData);
  }

  // Delete user
  deleteUser(userId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/users/${userId}`);
  }
}
