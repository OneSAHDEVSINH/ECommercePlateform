import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import '@angular/compiler';
import { AppModule } from './app/app.module';
import { AppComponent } from './app/app.component';
import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';

bootstrapApplication(AppComponent, appConfig)
  .catch((err) => console.error(err));
// import { bootstrapApplication, provideClientHydration } from '@angular/platform-browser';
// import { AppComponent } from './app/app.component';
// import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
// import { provideRouter } from '@angular/router';
// import { provideAnimations } from '@angular/platform-browser/animations';
// import { ApplicationConfig, importProvidersFrom } from '@angular/core';
// import '@angular/compiler';
// import { routes } from './app/app.routes';

// const appConfig: ApplicationConfig = {
//   providers: [
    
//   ]
// };

// bootstrapApplication(AppComponent, appConfig)
//   .catch(err => console.error(err));



//import { platformBrowser } from '@angular/platform-browser';
//import { StudentModule } from './app/app.module';
//import "@angular/compiler"
//platformBrowser().bootstrapModule(StudentModule, {
//  ngZoneEventCoalescing: true,
//})
//  .catch(err => console.error(err));
//import { bootstrapApplication } from '@angular/platform-browser';
//import { StudentComponent } from './app/app.component';
//import "@angular/compiler";

//bootstrapApplication(StudentComponent, {
//  ngZoneEventCoalescing: true,
//})
//  .catch(err => console.error(err));


//import { bootstrapApplication } from '@angular/platform-browser';
//import "@angular/compiler";
//import { provideHttpClient } from '@angular/common/http';
//import { importProvidersFrom } from '@angular/core';
//import { provideRouter } from '@angular/router';
//import { AppRoutingModule } from './app/app-routing.module';
//import { LoginComponent } from './app/admin/login/login.component';


//bootstrapApplication(LoginComponent, {
//  providers: [
//    provideHttpClient(),
//    importProvidersFrom(AppRoutingModule)
//  ]
//})
//  .catch(err => console.error(err));


