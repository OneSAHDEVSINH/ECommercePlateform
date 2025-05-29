import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { CountryComponent } from './country/country.component';
import { StateComponent } from './state/state.component';
import { CityComponent } from './city/city.component';
import { authGuard } from '../guards/auth.guard';
import { PageNotFoundComponent } from '../page-not-found/page-not-found.component';

const routes: Routes = [
  { 
    path: 'admin', 
    children: [
      {
        path: 'login',
        component: LoginComponent,
        title: 'Admin Login'
      },
      { 
        path: 'dashboard', 
        component: DashboardComponent,
        title: 'Admin Dashboard', 
        canActivate: [authGuard] 
      },
      { 
        path: 'countries', 
        component: CountryComponent,
        title: 'Country Component',
        canActivate: [authGuard] 
      },
      { 
        path: 'states', 
        component: StateComponent,
        title: 'State Component',
        canActivate: [authGuard] 
      },
      { 
        path: 'cities', 
        component: CityComponent,
        title: 'City Component',
        canActivate: [authGuard] 
      },
      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full'
      },
      {
        path: '**',
        component: PageNotFoundComponent,
        pathMatch: 'full',
        title: '404 - Page not found'
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AdminRoutingModule { }
