import { Directive, Input, TemplateRef, ViewContainerRef, OnInit, OnDestroy } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { Subscription, combineLatest } from 'rxjs';
import { filter } from 'rxjs/operators';
import { PermissionType } from '../models/role.model';
import { AuthorizationService } from '../services/authorization/authorization.service';
import { AuthService } from '../services/auth/auth.service';

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
    private authorizationService: AuthorizationService,
    private authService: AuthService
  ) { }

  ngOnInit() {
    // Subscribe to auth state changes
    this.subscription = this.authService.authStateChange$.subscribe(isLoggedIn => {
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

    const hasPermission = this.authorizationService.hasPermission(
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

  private clearView() {
    this.viewContainer.clear();
    this.hasView = false;
  }

  ngOnDestroy() {
    this.subscription?.unsubscribe();
  }
}
