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
            createdOn: response.user.createdOn,
            // Parse and store claims from token
            claims: this.parseClaimsFromToken(response.token)
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

  private parseClaimsFromToken(token: string): any[] {
    try {
      const decodedToken: any = jwtDecode(token);

      // Convert JWT claims to claim objects
      const claims = [];
      for (const key in decodedToken) {
        if (key !== 'exp' && key !== 'iat' && key !== 'nbf' && key !== 'iss' && key !== 'aud') {
          claims.push({
            type: key,
            value: decodedToken[key]
          });
        }
      }

      return claims;
    } catch (error) {
      console.error('Error parsing JWT claims:', error);
      return [];
    }
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

  isSuperAdmin(): boolean {
    const currentUser = this.currentUserSubject.getValue();
    if (!currentUser) return false;

    // Method 1: Check SuperAdmin claim from JWT
    const isSuperAdminClaim = currentUser.claims?.some(
      claim => claim.type === 'SuperAdmin' && claim.value === 'true'
    ) ?? false;

    // Method 2: Check email (fallback approach)
    const isSuperAdminEmail =
      typeof currentUser.email === 'string' &&
      currentUser.email.toLowerCase() === 'admin@admin.com';

    // Either method confirms SuperAdmin status
    return isSuperAdminClaim || isSuperAdminEmail;
  }

  hasAnyRole(): boolean {
    const currentUser = this.currentUserSubject.value;
    // If no user is logged in, return false
    if (!currentUser) return false;

    // If user is SuperAdmin, can continue regardless of roles
    if (this.isSuperAdmin()) return true;

    // For regular users, check if they have any roles
    return !!(currentUser.roles && currentUser.roles.length > 0);
  }

  getUserPermissions(): UserPermissionDto[] {
    return this.permissionsSubject.value;
  }

  getCurrentUser(): User | null {
    return this.currentUserSubject.getValue();
  }

  updatePermissionsData(permissions: UserPermissionDto[]): void {
    // Update permissions in memory
    this.permissionsSubject.next(permissions);

    // Update permissions in localStorage
    localStorage.setItem('userPermissions', JSON.stringify(permissions));

    console.log('Permissions updated:', permissions.length);
  }
}
