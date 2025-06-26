import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { LoginComponent } from './login/login.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { CountryComponent } from './country/country.component';
import { StateComponent } from './state/state.component';
import { CityComponent } from './city/city.component';
import { AdminLayoutComponent } from '../shared/admin-layout/admin-layout.component';
import { RoleComponent } from './role/role.component';
import { UserComponent } from './user/user.component';
import { AccessDeniedComponent } from '../shared/access-denied/access-denied.component';
import { ModuleComponent } from './module/module.component';

@NgModule({
  imports: [
    CommonModule,
    ReactiveFormsModule,
    LoginComponent,
    DashboardComponent,
    CountryComponent,
    StateComponent,
    CityComponent,
    AdminLayoutComponent,
    UserComponent,
    RoleComponent,
    AccessDeniedComponent,
    ModuleComponent
  ]
})
export class AdminModule { }
