import { Injectable } from '@angular/core';
import { AuthService } from '../auth/auth.service';
import { User } from '../../models/user.model';

@Injectable({ providedIn: 'root' })
export class PermissionService {
  private currentUser: User | null = null;

  constructor(private authService: AuthService) {
    // Subscribe to the user Observable to keep a local copy of the current user
    this.authService.currentUser$.subscribe(user => {
      this.currentUser = user;
    });
  }

  // Checks if the current user has a specific permission for a module
  hasPermission(module: string, permission: string): boolean {
    if (!this.currentUser || !this.currentUser.roles) return false;

    // Flatten all permissions from all roles
    const allPermissions = this.currentUser.roles.flatMap((role: any) => role.permissions || []);
    return allPermissions.some((perm: any) =>
      perm.moduleName?.toLowerCase() === module.toLowerCase() &&
      perm.permissionType?.toLowerCase() === permission.toLowerCase()
    );
  }
}
