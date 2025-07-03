import { Component, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { Router, RouterModule, ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth/auth.service';
import { PermissionDirective } from '../../directives/permission.directive';
import { PermissionType } from '../../models/role.model';
import { AuthorizationService } from '../../services/authorization/authorization.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
  standalone: true,
  imports: [CommonModule, RouterModule]
})
export class DashboardComponent implements OnInit, OnDestroy {
  userName: string = '';
  accessDeniedMessage: string | null = null;
  PermissionType = PermissionType;
  private permissionSubscription!: Subscription;
  private userSubscription!: Subscription;

  // Permission-dependent UI state
  canViewCountries: boolean = false;
  canViewStates: boolean = false;
  canViewCities: boolean = false;
  canViewUsers: boolean = false;
  canViewRoles: boolean = false;
  canViewModules: boolean = false;

  constructor(
    private authService: AuthService,
    public authorizationService: AuthorizationService,
    private router: Router,
    private route: ActivatedRoute,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    // Initial permission check
    this.checkPermissions();

    // Subscribe to global permission changes
    this.permissionSubscription = this.authorizationService.globalPermissionChange$.subscribe(() => {
      this.checkPermissions(); // Refresh permission-dependent state
      this.cdr.detectChanges(); // Force change detection
    });

    // Subscribe to user changes
    this.userSubscription = this.authService.currentUser$.subscribe(user => {
      if (user) {
        const firstName = user.firstName || user['firstName'] || '';
        const lastName = user.lastName || user['lastName'] || '';
        this.userName = `${firstName} ${lastName}`.trim() || user.email;
      }
    });
  }

  ngOnDestroy(): void {
    if (this.permissionSubscription) {
      this.permissionSubscription.unsubscribe();
    }
    if (this.userSubscription) {
      this.userSubscription.unsubscribe();
    }
  }

  // Method to check permissions
  private checkPermissions(): void {
    this.canViewCountries = this.authorizationService.hasPermission('countries', PermissionType.View);
    this.canViewStates = this.authorizationService.hasPermission('states', PermissionType.View);
    this.canViewCities = this.authorizationService.hasPermission('cities', PermissionType.View);
    this.canViewUsers = this.authorizationService.hasPermission('users', PermissionType.View);
    this.canViewRoles = this.authorizationService.hasPermission('roles', PermissionType.View);
    this.canViewModules = this.authorizationService.hasPermission('modules', PermissionType.View);

    console.log('Dashboard permissions updated:', {
      canViewCountries: this.canViewCountries,
      canViewStates: this.canViewStates,
      canViewCities: this.canViewCities,
      canViewUsers: this.canViewUsers,
      canViewRoles: this.canViewRoles,
      canViewModules: this.canViewModules
    });
  }
}
