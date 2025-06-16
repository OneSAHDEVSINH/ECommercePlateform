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
  private subscriptions: Subscription = new Subscription();
  private routerSubscription: Subscription = new Subscription();

  constructor(
    private templateRef: TemplateRef<any>,
    private viewContainer: ViewContainerRef,
    private authorizationService: AuthorizationService,
    private authService: AuthService,
    private router: Router
  ) { }

  ngOnInit() {

    // Subscribe to both auth state changes and route changes
    this.subscriptions.add(
      combineLatest([
        this.authService.authStateChange$,
        this.router.events.pipe(filter(event => event instanceof NavigationEnd))
      ]).subscribe(([isLoggedIn]) => {
        if (isLoggedIn) {
          this.checkPermission();
        } else {
          // If not logged in, clear view
          this.viewContainer.clear();
          this.hasView = false;
        }
      })
    );

    this.checkPermission();

    //// Listen for route changes to re-evaluate permissions
    //this.routerSubscription = this.router.events
    //  .pipe(filter(event => event instanceof NavigationEnd))
    //  .subscribe(() => this.checkPermission());
  }

  private checkPermission() {
    if (!this.appPermission || !this.authService.isAuthenticated()) {
      this.viewContainer.clear();
      this.hasView = false;
      return;
    }

    // Check permission through the authorization service
    this.subscriptions.add(
      this.authorizationService
        .checkPermission(this.appPermission.moduleRoute, this.appPermission.type)
        .subscribe(hasPermission => {
          if (hasPermission && !this.hasView) {
            this.viewContainer.createEmbeddedView(this.templateRef);
            this.hasView = true;
          } else if (!hasPermission && this.hasView) {
            this.viewContainer.clear();
            this.hasView = false;
          }
        })
    );
  }

  ngOnDestroy() {
    if (this.subscriptions) {
      this.subscriptions.unsubscribe();
    }
    if (this.routerSubscription) {
      this.routerSubscription.unsubscribe();
    }
  }
}
