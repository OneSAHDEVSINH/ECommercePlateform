import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
//import { HttpClientModule } from '@angular/common/http';
//routing.module';
import { LoginComponent } from './login/login.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { CountryComponent } from './country/country.component';
import { StateComponent } from './state/state.component';
import { CityComponent } from './city/city.component';
import { AdminLayoutComponent } from '../shared/admin-layout/admin-layout.component';
import { RoleManagementComponent } from './role-management/role-management.component';
import { UserComponent } from './user/user.component';

@NgModule({
  imports: [
    CommonModule,
    //AdminRoutingModule,
    ReactiveFormsModule,
    //HttpClientModule,
    // Import standalone components
    LoginComponent,
    DashboardComponent,
    CountryComponent,
    StateComponent,
    CityComponent,
    AdminLayoutComponent,
    UserComponent,
    RoleManagementComponent
  ]
})
export class AdminModule { }
