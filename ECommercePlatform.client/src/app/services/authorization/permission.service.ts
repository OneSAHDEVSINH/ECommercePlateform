import { Injectable } from '@angular/core';
import { AuthService } from '../auth/auth.service';
import { User } from '../../models/user.model';
import { PermissionType } from '../../models/role.model';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class PermissionService {
  private currentUser: User | null = null;
  private permissionsLoaded = new BehaviorSubject<boolean>(false);

  constructor(private authService: AuthService) {
    // Subscribe to the user Observable to keep a local copy of the current user
    this.authService.currentUser$.subscribe(user => {
      this.currentUser = user;
      this.permissionsLoaded.next(!!user);
    });
  }

  // Wait until permissions are loaded
  get permissionsReady(): Observable<boolean> {
    return this.permissionsLoaded.asObservable();
  }

  // Checks if the current user has a specific permission for a module
  hasPermission(moduleName: string, permissionType: string | PermissionType): boolean {
    if (!this.currentUser || !this.currentUser.roles) return false;

    // Super admin check
    if (this.isSuperAdmin()) {
      return true;
    }

    // Standard permission check
    const normalized = {
      module: moduleName.toLowerCase(),
      permission: permissionType.toString().toLowerCase()
    };

    // Flatten all permissions from all roles
    const allPermissions = this.currentUser.roles.flatMap((role: any) => role.permissions || []);
    return allPermissions.some((perm: any) =>
      perm.moduleName?.toLowerCase() === normalized.module &&
      perm.permissionType?.toLowerCase() === normalized.permission
    );
  }

  hasViewPermission(moduleName: string): boolean {
    return this.hasPermission(moduleName, PermissionType.View);
  }

  hasCreatePermission(moduleName: string): boolean {
    return this.hasPermission(moduleName, PermissionType.Add);
  }

  hasEditPermission(moduleName: string): boolean {
    return this.hasPermission(moduleName, PermissionType.Edit);
  }

  hasDeletePermission(moduleName: string): boolean {
    return this.hasPermission(moduleName, PermissionType.Delete);
  }

  isSuperAdmin(): boolean {
    return this.authService.isSuperAdmin();
  }
}
