import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './admin/login/login.component';
import { DashboardComponent } from './admin/dashboard/dashboard.component';
import { CountryComponent } from './admin/country/country.component';
import { StateComponent } from './admin/state/state.component';
import { CityComponent } from './admin/city/city.component';

const routes: Routes = [
  { path: '', redirectTo: '/admin/login', pathMatch: 'full' },
  { path: 'admin/login', component: LoginComponent },
  { 
    path: 'admin/dashboard', 
    component: DashboardComponent,
    children: [
      { path: '', redirectTo: 'country', pathMatch: 'full' },
      { path: 'country', component: CountryComponent },
      { path: 'state', component: StateComponent },
      { path: 'city', component: CityComponent }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
