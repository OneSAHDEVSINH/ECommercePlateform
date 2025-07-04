import { ApplicationConfig, ErrorHandler, APP_INITIALIZER } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideAnimations } from '@angular/platform-browser/animations';
import { routes } from './app.routes';
import { ErrorHandlerService } from './services/general/error-handler.service';
import { NavigationErrorHandlerService } from './services/general/navigation-error-handler.service';
import { errorInterceptor } from './services/general/http-error.interceptor';
import { PermissionRefreshService } from './services/general/permission-refresh.service'

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    provideHttpClient(withInterceptors([
      errorInterceptor,
      (req, next) => {
        const token = localStorage.getItem('token');
        if (token) {
          req = req.clone({
            setHeaders: {
              Authorization: `Bearer ${token}`
            }
          });
        }
        return next(req);
      }
    ])),
    PermissionRefreshService,
    provideAnimations(),
    { provide: ErrorHandler, useClass: ErrorHandlerService },
    {
      provide: APP_INITIALIZER,
      useFactory: (navErrorHandler: NavigationErrorHandlerService) => {
        return () => navErrorHandler.initialize();
      },
      deps: [NavigationErrorHandlerService],
      multi: true
    }
  ]
};
