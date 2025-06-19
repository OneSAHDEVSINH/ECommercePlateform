import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject, tap, of, map } from 'rxjs';
import { LoginRequest, LoginResponse, User, UserPermissionDto } from '../../models/user.model';
import { environment } from '../../../environments/environment';
import { PagedRequest, PagedResponse } from '../../models/pagination.model';
import { PermissionType, Role } from '../../models/role.model';
import { jwtDecode } from 'jwt-decode';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private currentUserSubject = new BehaviorSubject<User | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  private permissionsSubject = new BehaviorSubject<UserPermissionDto[]>([]);
  public permissions$ = this.permissionsSubject.asObservable();

  private authStateChangeSubject = new BehaviorSubject<boolean>(false);
  public authStateChange$ = this.authStateChangeSubject.asObservable();

  private apiUrl = environment.apiUrl;

  constructor(
    private http: HttpClient,
    private router: Router
  ) {
    this.loadStoredAuth();
  }

  private loadStoredAuth(): void {
    const token = this.getToken();
    const userJson = localStorage.getItem('currentUser');
    const permissionsJson = localStorage.getItem('userPermissions');

    if (token && userJson) {
      try {
        const user = JSON.parse(userJson) as User;
        const permissions = permissionsJson ? JSON.parse(permissionsJson) : [];

        this.currentUserSubject.next(user);
        this.permissionsSubject.next(permissions);
        this.authStateChangeSubject.next(true);
      } catch (error) {
        console.error('Error loading stored auth:', error);
        this.clearAuth();
      }
    }
  }

  login(credentials: LoginRequest): Observable<LoginResponse> {
    return this.http.post<any>(`${this.apiUrl}/Auth/login`, credentials)
      .pipe(
        map(response => {
          console.log('Login response:', response);

          // Store token
          localStorage.setItem('token', response.token);

          // Create user object
          const user: User = {
            id: response.user.id,
            firstName: response.user.firstName || '',
            lastName: response.user.lastName || '',
            email: response.user.email || '',
            roles: response.user.roles || [],
            isActive: response.user.isActive !== undefined ? response.user.isActive : true, // Fix here
            phoneNumber: response.user.phoneNumber,
            bio: response.user.bio,
            createdOn: response.user.createdOn
          };

          // Store user and permissions
          localStorage.setItem('currentUser', JSON.stringify(user));

          if (response.permissions) {
            localStorage.setItem('userPermissions', JSON.stringify(response.permissions));
            this.permissionsSubject.next(response.permissions);
          }

          // Update subjects
          this.currentUserSubject.next(user);
          this.authStateChangeSubject.next(true);

          return {
            token: response.token,
            user: user,
            permissions: response.permissions
          } as LoginResponse;
        })
      );
  }

  logout(): void {
    this.clearAuth();
    this.router.navigate(['/admin/login']);
  }

  private clearAuth(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('currentUser');
    localStorage.removeItem('userPermissions');

    this.currentUserSubject.next(null);
    this.permissionsSubject.next([]);
    this.authStateChangeSubject.next(false);
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  isAuthenticated(): boolean {
    const token = this.getToken();
    if (!token) return false;

    try {
      const decodedToken: any = jwtDecode(token);
      const expirationDate = new Date(decodedToken.exp * 1000);
      return expirationDate > new Date();
    } catch {
      return false;
    }
  }

  hasAnyRole(): boolean {
    const user = this.currentUserSubject.value;
    return !!(user?.roles && user.roles.length > 0);
  }

  isSuperAdmin(): boolean {
    const token = this.getToken();
    if (!token) return false;

    try {
      const decodedToken: any = jwtDecode(token);
      return decodedToken.SuperAdmin === 'true' ||
        decodedToken.role === 'SuperAdmin';
    } catch {
      return false;
    }
  }

  getUserPermissions(): UserPermissionDto[] {
    return this.permissionsSubject.value;
  }
}

//@Injectable({
//  providedIn: 'root'
//})
//export class AuthService {
//  private currentUserSubject = new BehaviorSubject<User | null>(null);
//  public currentUser$ = this.currentUserSubject.asObservable();

//  // Authentication state change subject
//  private authStateChangeSubject = new BehaviorSubject<boolean>(false);
//  public authStateChange$ = this.authStateChangeSubject.asObservable();

//  private apiUrl = environment.apiUrl;
//  //private apiUrl = '/Auth';
//  //private apiUrl = 'https://localhost:44362/Auth';


//  // Development mode flag - REMOVE IN PRODUCTION
//  private devMode = false;

//  constructor(private http: HttpClient) {
//    // Check if user is already logged in on initialization
//    this.loadCurrentUser();

//    // Auto-login for development - REMOVE IN PRODUCTION
//    if (this.devMode && !this.currentUserSubject.value) {
//      this.simulateLogin();
//    }
//  }

//  // Development only - REMOVE IN PRODUCTION
//  private simulateLogin(): void {
//    const mockUser: User = {
//      id: '1',
//      firstName: 'Admin',
//      lastName: 'User',
//      email: 'admin@admin.com',
//      //role: UserRole.Admin,
//      roles: [{ name: 'Admin' } as Role],
//      isActive: true
//    };

//    localStorage.setItem('token', 'fake-jwt-token');
//    localStorage.setItem('currentUser', JSON.stringify(mockUser));
//    this.currentUserSubject.next(mockUser);
//    this.authStateChangeSubject.next(true);
//    console.warn('DEV MODE: Auto-login enabled. Remove before production!');
//  }

//  login(credentials: LoginRequest): Observable<LoginResponse> {
//    // For development - REMOVE IN PRODUCTION
//    if (this.devMode) {
//      const mockUser: User = {
//        id: '1',
//        firstName: 'Admin',
//        lastName: 'User',
//        email: credentials.email,
//        //role: UserRole.Admin,
//        roles: [{ name: 'Admin' } as Role],
//        isActive: true
//      };

//      const response: LoginResponse = {
//        token: 'fake-jwt-token',
//        user: mockUser
//      };

//      localStorage.setItem('token', response.token);
//      localStorage.setItem('currentUser', JSON.stringify(response.user));
//      this.currentUserSubject.next(response.user);
//      this.authStateChangeSubject.next(true);
//      return of(response);
//    }


//    // Real implementation for production
//    return this.http.post<LoginResponse>(`${this.apiUrl}/Auth/login`, credentials)
//      .pipe(
//        tap(response => {
//          console.log('Raw API response:', response);

//          // Store token in local storage
//          localStorage.setItem('token', response.token);

//          // Check if user data exists in the response
//          if (response.user) {
//            // Create a properly formatted user object
//            const user: User = {
//              id: response.user.id,
//              firstName: response.user.firstName || '',
//              lastName: response.user.lastName || '',
//              email: response.user.email || '',
//              roles: response.user.roles || [],
//              isActive: response.user.isActive || true
//            };

//            // Store user data
//            localStorage.setItem('currentUser', JSON.stringify(response.user));

//            // Update current user subject
//            this.currentUserSubject.next(user);
//            this.authStateChangeSubject.next(true);
//          } else {
//            // Extract user info from the JWT token
//            try {
//              const decodedToken: any = jwtDecode(response.token);

//              // Create a minimal user object from token claims
//              const userFromToken: User = {
//                id: decodedToken.nameid || '',
//                email: decodedToken.email || '',
//                firstName: '',
//                lastName: '',
//                roles: [{ name: decodedToken.role }],
//                isActive: true
//              };

//              // Store and use this user data
//              localStorage.setItem('currentUser', JSON.stringify(userFromToken));
//              this.currentUserSubject.next(userFromToken);
//              this.authStateChangeSubject.next(true);
//            } catch (error) {
//              console.error('Error decoding JWT token:', error);
//              // Re-throw to trigger error handler
//              throw new Error('Unable to retrieve user information');
//            }
//          }
//        })
//      );
//  }

//  logout(): void {
//    // Remove token and user data from local storage
//    localStorage.removeItem('token');
//    localStorage.removeItem('currentUser');

//    // Update current user subject
//    this.currentUserSubject.next(null);
//    this.authStateChangeSubject.next(false);
//  }

//  private loadCurrentUser(): void {
//    const userJson = localStorage.getItem('currentUser');
//    if (userJson) {
//      const user = JSON.parse(userJson) as User;
//      this.currentUserSubject.next(user);
//      this.authStateChangeSubject.next(true);
//    } else {
//      this.authStateChangeSubject.next(false);
//    }
//  }

//  getToken(): string | null {
//    return localStorage.getItem('token');
//  }

//  isAuthenticated(): boolean {
//    return !!this.getToken();
//  }

//  isAdmin(): boolean {
//    const user = this.currentUserSubject.value;
//    if (!user || !user.roles) return false;

//    // Check if user has the Admin role
//    return user.roles.some((role: Role) => role.name === 'Admin');
//  }

//  isSuperAdmin(): boolean {
//    // First check if user is admin at all
//    if (!this.isAdmin()) return false;

//    // Then check for SuperAdmin claim in the token
//    const token = this.getToken();
//    if (!token) return false;

//    try {
//      const decodedToken: any = jwtDecode(token);
//      return decodedToken.SuperAdmin === 'true';
//    } catch (error) {
//      console.error('Error decoding JWT token:', error);
//      return false;
//    }
//  }

//  // Check if user has permission
//  hasPermission(moduleRoute: string, permissionType: PermissionType): boolean {
//    // This is a placeholder. Implement your actual permission check logic
//    // For now, returning true to make development easier
//    return true;
//  }

//  // Get users with paging (or use a separate UserService)
//  getUsers(pageRequest: PagedRequest): Observable<PagedResponse<User>> {
//    return this.http.get<PagedResponse<User>>(`${this.apiUrl}/users`, {
//      params: { ...pageRequest as any }
//    });
//  }

//  // Create user
//  createUser(userData: User): Observable<User> {
//    return this.http.post<User>(`${this.apiUrl}/users`, userData);
//  }

//  // Update user
//  updateUser(userId: string, userData: User): Observable<void> {
//    return this.http.put<void>(`${this.apiUrl}/users/${userId}`, userData);
//  }

//  // Delete user
//  deleteUser(userId: string): Observable<void> {
//    return this.http.delete<void>(`${this.apiUrl}/users/${userId}`);
//  }

//  // Development mode methods
//  enableDevMode(enable: boolean): void {
//    this.devMode = enable;
//    console.log(`Dev mode ${enable ? 'enabled' : 'disabled'}`);
//  }

//  isDevMode(): boolean {
//    return this.devMode;
//  }
//}
