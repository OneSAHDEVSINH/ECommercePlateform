import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule, provideClientHydration } from '@angular/platform-browser';
import { RouterModule } from '@angular/router';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
//import { LoginComponent } from './admin/login/login.component';

@NgModule({
  declarations: [
    // No declarations since AppComponent is standalone
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    AppRoutingModule,
    RouterModule,
    AppComponent
  ],
  providers: [provideClientHydration()],
  bootstrap: []
})
export class AppModule { }
