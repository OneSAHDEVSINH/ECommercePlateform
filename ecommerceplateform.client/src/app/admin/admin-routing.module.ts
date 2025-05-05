import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { CountryComponent } from './country/country.component';
import { StateComponent } from './state/state.component';
import { CityComponent } from './city/city.component';
import { authGuard } from '../guards/auth.guard';

const routes: Routes = [
  { 
    path: 'admin', 
    children: [
      { path: 'login', component: LoginComponent },
      { 
        path: 'dashboard', 
        component: DashboardComponent, 
        canActivate: [authGuard] 
      },
      { 
        path: 'countries', 
        component: CountryComponent, 
        canActivate: [authGuard] 
      },
      { 
        path: 'states', 
        component: StateComponent, 
        canActivate: [authGuard] 
      },
      { 
        path: 'cities', 
        component: CityComponent, 
        canActivate: [authGuard] 
      },
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AdminRoutingModule { }
