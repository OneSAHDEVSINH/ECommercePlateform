import { Routes } from '@angular/router';
import { LoginComponent } from './admin/login/login.component';
import { DashboardComponent } from './admin/dashboard/dashboard.component';
import { CountryComponent } from './admin/country/country.component';
import { StateComponent } from './admin/state/state.component';
import { CityComponent } from './admin/city/city.component';
import { authGuard } from './guards/auth.guard';
import { PageNotFoundComponent } from './page-not-found/page-not-found.component';

export const routes: Routes = [
  { path: '', redirectTo: 'admin/login', pathMatch: 'full' },
  { path: 'admin/login', component: LoginComponent, title: 'Admin Login' },
  { 
    path: 'admin/dashboard', 
    component: DashboardComponent,
    title: 'Admin Dashboard',
    canActivate: [authGuard] 
  },
  { 
    path: 'admin/countries', 
    component: CountryComponent,
    title: 'Country Component',
    canActivate: [authGuard] 
  },
  { 
    path: 'admin/states', 
    component: StateComponent,
    title: 'State Component',
    canActivate: [authGuard] 
  },
  { 
    path: 'admin/cities', 
    component: CityComponent,
    title: 'City Component',
    canActivate: [authGuard] 
  },
  { path: '**', component: PageNotFoundComponent, pathMatch: 'full', title: '404 - Page not found' }
];
