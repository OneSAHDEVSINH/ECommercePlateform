import { Routes, UrlSegment } from '@angular/router';
import { LoginComponent } from './admin/login/login.component';
import { DashboardComponent } from './admin/dashboard/dashboard.component';
import { CountryComponent } from './admin/country/country.component';
import { StateComponent } from './admin/state/state.component';
import { CityComponent } from './admin/city/city.component';
import { authGuard } from './guards/auth.guard';
import { PageNotFoundComponent } from './page-not-found/page-not-found.component';
import { AdminLayoutComponent } from './shared/admin-layout/admin-layout.component';
import { RoleManagementComponent } from './admin/role-management/role-management.component';
import { UserManagementComponent } from './admin/user-management/user-management.component';
import { PermissionGuard } from './guards/permission.guard';
import { PermissionType } from './models/role.model';
import { ModuleManagementComponent } from './admin/module-management/module-management.component';

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
            path: 'dashboard',
            component: DashboardComponent,
            title: 'Admin Dashboard',
            // Dashboard is exempt from specific permission checks
            data: { exempt: true }
          },
          {
            path: 'countries',
            component: CountryComponent,
            title: 'Country Management',
            data: {
              moduleRoute: 'countries',
              permission: PermissionType.VIEW
            }
          },
          {
            path: 'states',
            component: StateComponent,
            title: 'State Management',
            data: {
              moduleRoute: 'states',
              permission: PermissionType.VIEW
            }
          },
          {
            path: 'cities',
            component: CityComponent,
            title: 'City Management',
            data: {
              moduleRoute: 'cities',
              permission: PermissionType.VIEW
            }
          },
          {
            path: 'roles',
            component: RoleManagementComponent,
            title: 'Role Management',
            data: {
              moduleRoute: 'roles',
              permission: PermissionType.VIEW,
              adminOnly: true // Special flag for admin-only sections
            }
          },
          {
            path: 'users',
            component: UserManagementComponent,
            title: 'User Management',
            data: {
              moduleRoute: 'users',
              permission: PermissionType.VIEW,
              adminOnly: true
            }
          },
          {
            path: 'modules',
            component: ModuleManagementComponent,
            title: 'Module Management',
            data: {
              moduleRoute: 'modules',
              permission: PermissionType.VIEW,
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

//export const routes: Routes = [
//  {
//    matcher: malformedUrlMatcher,
//    component: PageNotFoundComponent
//  },
//  {
//    path: '',
//    redirectTo: 'admin/login',
//    pathMatch: 'full'
//  },
//  {
//    path: 'admin',
//    children: [
//      {
//        path: 'login',
//        component: LoginComponent,
//        title: 'Admin Login'
//      },
//      {
//        path: '',
//        component: AdminLayoutComponent,
//        canActivate: [PermissionGuard],
//        children: [
//          {
//            path: 'dashboard',
//            component: DashboardComponent,
//            title: 'Admin Dashboard',
//            // Dashboard is exempt from specific permission checks
//            data: { exempt: true }
//          },
//          {
//            path: 'countries',
//            component: CountryComponent,
//            title: 'Country Management',
//            data: {
//              moduleRoute: 'countries',
//              permission: PermissionType.VIEW
//            }
//          },
//          {
//            path: 'states',
//            component: StateComponent,
//            title: 'State Management',
//            data: {
//              moduleRoute: 'states',
//              permission: PermissionType.VIEW
//            }
//          },
//          {
//            path: 'cities',
//            component: CityComponent,
//            title: 'City Management',
//            data: {
//              moduleRoute: 'cities',
//              permission: PermissionType.VIEW
//            }
//          },
//          {
//            path: 'roles',
//            component: RoleManagementComponent,
//            title: 'Role Management',
//            data: {
//              moduleRoute: 'roles',
//              permission: PermissionType.VIEW,
//              adminOnly: true // Special flag for admin-only sections
//            }
//          },
//          {
//            path: 'users',
//            component: UserManagementComponent,
//            title: 'User Management',
//            data: {
//              moduleRoute: 'users',
//              permission: PermissionType.VIEW,
//              adminOnly: true
//            }
//          },
//          {
//            path: '',
//            redirectTo: 'dashboard',
//            pathMatch: 'full'
//          }
//        ]
//      }
//    ]
//  },
//  {
//    path: '**',
//    component: PageNotFoundComponent,
//    title: '404 - Page not found'
//  }
//];

//export const routes: Routes = [

//  {
//    matcher: malformedUrlMatcher,
//    component: PageNotFoundComponent
//  },
//  {
//    path: '',
//    redirectTo: 'admin/login',
//    pathMatch: 'full'
//  },
//  {
//    path: 'admin',
//    children: [
//      {
//        path: 'login',
//        component: LoginComponent,
//        title: 'Admin Login'
//      },
//      {
//        path: '',
//        component: AdminLayoutComponent,
//        canActivate: [PermissionGuard],
//        children: [
//          {
//            path: 'dashboard',
//            component: DashboardComponent,
//            title: 'Admin Dashboard'
//          },
//          {
//            path: 'countries',
//            component: CountryComponent,
//            title: 'Country Management'
//          },
//          {
//            path: 'states',
//            component: StateComponent,
//            title: 'State Management'
//          },
//          {
//            path: 'cities',
//            component: CityComponent,
//            title: 'City Management'
//          },
//          {
//            path: 'roles',
//            component: RoleManagementComponent,
//            title: 'Role Management'
//          },
//          {
//            path: 'users',
//            component: UserManagementComponent,
//            title: 'User Management'
//          },
//          {
//            path: '',
//            redirectTo: 'dashboard',
//            pathMatch: 'full'
//          }
//        ]
//      }
//    ]
//  },
//  {
//    path: '**',
//    component: PageNotFoundComponent,
//    title: '404 - Page not found'
//  }
//];


//export const routes: Routes = [
//  {
//    path: '',
//    redirectTo: 'admin/login',
//    pathMatch: 'full'
//  },
//  {
//    path: 'admin/login',
//    component: LoginComponent,
//    title: 'Admin Login'
//  },
//  {
//    path: 'admin/dashboard',
//    component: DashboardComponent,
//    title: 'Admin Dashboard',
//    canActivate: [authGuard]
//  },
//  {
//    path: 'admin/countries',
//    component: CountryComponent,
//    title: 'Country Component',
//    canActivate: [authGuard]
//  },
//  {
//    path: 'admin/states',
//    component: StateComponent,
//    title: 'State Component',
//    canActivate: [authGuard]
//  },
//  {
//    path: 'admin/cities',
//    component: CityComponent,
//    title: 'City Component',
//    canActivate: [authGuard]
//  },
//  {
//    path: '**',
//    component: PageNotFoundComponent,
//    pathMatch: 'full',
//    title: '404 - Page not found'
//  }
//];
