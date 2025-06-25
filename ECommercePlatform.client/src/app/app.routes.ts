import { Routes, UrlSegment } from '@angular/router';
import { LoginComponent } from './admin/login/login.component';
import { DashboardComponent } from './admin/dashboard/dashboard.component';
import { CountryComponent } from './admin/country/country.component';
import { StateComponent } from './admin/state/state.component';
import { CityComponent } from './admin/city/city.component';
import { PageNotFoundComponent } from './page-not-found/page-not-found.component';
import { AdminLayoutComponent } from './shared/admin-layout/admin-layout.component';
import { RoleComponent } from './admin/role/role.component';
import { UserComponent } from './admin/user/user.component';
import { PermissionGuard } from './guards/permission.guard';
import { PermissionType } from './models/role.model';
import { ModuleComponent } from './admin/module/module.component';
import { AccessDeniedComponent } from './shared/access-denied.component';

// Custom matcher to catch malformed URLs
function malformedUrlMatcher(url: UrlSegment[]) {
  const fullUrl = url.map(segment => segment.path).join('/');

  // Check if URL contains malformed encoding
  if (fullUrl.includes('%F') ||
    fullUrl.includes('%f') ||

    fullUrl.match(/%[0-9A-F]([^0-9A-F]|$)/i)) {
    return {
      consumed: url,
      posParams: {
        malformedUrl: new UrlSegment(fullUrl, {})
      }
    };
  }

  return null;
}

export const routes: Routes = [
  {
    matcher: malformedUrlMatcher,
    component: PageNotFoundComponent
  },
  {
    path: '',
    redirectTo: 'admin/login',
    pathMatch: 'full'
  },
  {
    path: 'admin',
    children: [

      {
        path: 'login',
        component: LoginComponent,
        title: 'Admin Login'
      },
      {
        path: '',
        component: AdminLayoutComponent,
        canActivate: [PermissionGuard],
        children: [
          {
            path: 'access-denied',
            component: AccessDeniedComponent,
            title: 'Access Denied',
            data: { exempt: true }
          },
          {
            path: 'dashboard',
            component: DashboardComponent,
            title: 'Admin Dashboard',
            // Dashboard is exempt from specific permission checks
            //data: { exempt: true }
            data: {
              moduleRoute: 'dashboard',
              permission: PermissionType.View
            }
          },
          {
            path: 'countries',
            component: CountryComponent,
            title: 'Country Management',
            canActivate: [PermissionGuard],
            data: {
              moduleRoute: 'countries',
              permission: [PermissionType.View,
                PermissionType.AddEdit,
                PermissionType.Delete]
            }
          },
          {
            path: 'states',
            component: StateComponent,
            title: 'State Management',
            canActivate: [PermissionGuard],
            data: {
              moduleRoute: 'states',
              permission: PermissionType.View
            }
          },
          {
            path: 'cities',
            component: CityComponent,
            title: 'City Management',
            canActivate: [PermissionGuard],
            data: {
              moduleRoute: 'cities',
              permission: PermissionType.View
            }
          },
          {
            path: 'roles',
            component: RoleComponent,
            title: 'Role Management',
            canActivate: [PermissionGuard],
            data: {
              moduleRoute: 'roles',
              permission: PermissionType.View,
              adminOnly: true // Special flag for admin-only sections
            }
          },
          {
            path: 'users',
            component: UserComponent,
            title: 'User Management',
            canActivate: [PermissionGuard],
            data: {
              moduleRoute: 'users',
              permission: PermissionType.View,
              adminOnly: true
            }
          },
          {
            path: 'modules',
            component: ModuleComponent,
            title: 'Module Management',
            canActivate: [PermissionGuard],
            data: {
              moduleRoute: 'modules',
              permission: PermissionType.View,
              adminOnly: true
            }
          },
          {
            path: '',
            redirectTo: 'dashboard',
            pathMatch: 'full'
          }
        ]
      }
    ]
  },
  {
    path: '**',
    component: PageNotFoundComponent,
    title: '404 - Page not found'
  }
];
