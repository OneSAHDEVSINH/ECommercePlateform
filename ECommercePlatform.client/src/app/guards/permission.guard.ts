import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { PermissionService } from '../services/authorization/permission.service';

@Injectable({ providedIn: 'root' })
export class PermissionGuard implements CanActivate {
  constructor(private permissionService: PermissionService, private router: Router) { }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    const module = route.data['module'];
    const permission = route.data['permission'];
    if (this.permissionService.hasPermission(module, permission)) {
      return true;
    }
    // Optionally redirect to a not-authorized page
    //this.router.navigate(['/not-authorized']);
    return false;
  }
}
