import { Routes } from '@angular/router';
import { LoginComponent } from './admin/login/login.component';
import { DashboardComponent } from './admin/dashboard/dashboard.component';
import { CountryComponent } from './admin/country/country.component';
import { StateComponent } from './admin/state/state.component';
import { CityComponent } from './admin/city/city.component';
import { authGuard } from './guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'admin/login', pathMatch: 'full' },
  { path: 'admin/login', component: LoginComponent },
  { 
    path: 'admin/dashboard', 
    component: DashboardComponent, 
    canActivate: [authGuard] 
  },
  { 
    path: 'admin/countries', 
    component: CountryComponent, 
    canActivate: [authGuard] 
  },
  { 
    path: 'admin/states', 
    component: StateComponent, 
    canActivate: [authGuard] 
  },
  { 
    path: 'admin/cities', 
    component: CityComponent, 
    canActivate: [authGuard] 
  }
];
