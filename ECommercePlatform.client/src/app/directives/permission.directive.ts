import { Directive, Input, TemplateRef, ViewContainerRef, OnInit, OnDestroy } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { Subscription, combineLatest } from 'rxjs';
import { filter, distinctUntilChanged, debounceTime } from 'rxjs/operators';
import { PermissionType } from '../models/role.model';
import { AuthorizationService } from '../services/authorization/authorization.service';
import { AuthService } from '../services/auth/auth.service';
import { PermissionNotificationService } from '../services/general/permission-notification.service';

@Directive({
  selector: '[appPermission]',
  standalone: true
})
export class PermissionDirective implements OnInit, OnDestroy {
  @Input() appPermission!: { moduleRoute: string, type: PermissionType };

  private hasView = false;
  private subscription?: Subscription;

  constructor(
    private templateRef: TemplateRef<any>,
    private viewContainer: ViewContainerRef,
    public authorizationService: AuthorizationService,
    private authService: AuthService,
    private permissionNotificationService: PermissionNotificationService
  ) { }

  ngOnInit() {
    // Enhanced reactive subscription
    this.subscription = combineLatest([
      this.authService.authStateChange$,
      this.authService.permissions$,
      this.authorizationService.globalPermissionChange$
    ]).pipe(
      debounceTime(100), // Prevent excessive updates
      distinctUntilChanged()
    ).subscribe(([isLoggedIn]) => {
      if (isLoggedIn) {
        this.updateView();
      } else {
        this.clearView();
      }
    });

    // Initial check
    if (this.authService.isAuthenticated()) {
      this.updateView();
    }
  }

  private updateView() {
    if (!this.appPermission) {
      this.clearView();
      return;
    }

    // Use synchronous check for immediate response
    const hasPermission = this.authorizationService.hasPermissionSync(
      this.appPermission.moduleRoute,
      this.appPermission.type
    );

    if (hasPermission && !this.hasView) {
      this.viewContainer.createEmbeddedView(this.templateRef);
      this.hasView = true;
    } else if (!hasPermission && this.hasView) {
      this.clearView();
    }
  }

  private addInteractionNotification(element: HTMLElement, moduleRoute: string, permissionType: PermissionType): void {
    element.addEventListener('click', (e) => {
      e.preventDefault();
      e.stopPropagation();

      this.permissionNotificationService.showPermissionError(
        moduleRoute,
        this.permissionTypeToString(permissionType)
      );
    });
  }

  // Helper method
  private permissionTypeToString(type: PermissionType): string {
    switch (type) {
      case PermissionType.View: return 'View';
      case PermissionType.AddEdit: return 'AddEdit';
      case PermissionType.Delete: return 'Delete';
      default: return 'access';
    }
  }

  private clearView() {
    this.viewContainer.clear();
    this.hasView = false;
  }

  ngOnDestroy() {
    this.subscription?.unsubscribe();
  }
}
