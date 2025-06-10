import { Directive, Input, TemplateRef, ViewContainerRef, OnInit, OnDestroy } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { Subscription } from 'rxjs';
import { filter } from 'rxjs/operators';
import { PermissionType } from '../models/role.model';
import { AuthorizationService } from '../services/authorization/authorization.service';

@Directive({
  selector: '[appPermission]',
  standalone: true
})
export class PermissionDirective implements OnInit, OnDestroy {
  @Input() appPermission!: { moduleRoute: string, type: PermissionType };

  private hasView = false;
  private subscription: Subscription = new Subscription();
  private routerSubscription: Subscription = new Subscription();

  constructor(
    private templateRef: TemplateRef<any>,
    private viewContainer: ViewContainerRef,
    private authorizationService: AuthorizationService,
    private router: Router
  ) { }

  ngOnInit() {
    this.checkPermission();

    // Listen for route changes to re-evaluate permissions
    this.routerSubscription = this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe(() => this.checkPermission());
  }

  private checkPermission() {
    if (!this.appPermission) {
      return;
    }

    // Check permission through the authorization service
    this.subscription = this.authorizationService
      .checkPermission(this.appPermission.moduleRoute, this.appPermission.type)
      .subscribe(hasPermission => {
        if (hasPermission && !this.hasView) {
          this.viewContainer.createEmbeddedView(this.templateRef);
          this.hasView = true;
        } else if (!hasPermission && this.hasView) {
          this.viewContainer.clear();
          this.hasView = false;
        }
      });
  }

  ngOnDestroy() {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
    if (this.routerSubscription) {
      this.routerSubscription.unsubscribe();
    }
  }
}

//import { Directive, Input, TemplateRef, ViewContainerRef } from '@angular/core';
//import { PermissionService } from '../services/authorization/permission.service';

//@Directive({
//  selector: '[appHasPermission]'
//})
//export class PermissionDirective {
//  @Input('appHasPermission') set permission([module, permission]: [string, string]) {
//    if (this.permissionService.hasPermission(module, permission)) {
//      this.viewContainer.createEmbeddedView(this.templateRef);
//    } else {
//      this.viewContainer.clear();
//    }
//  }

//  constructor(
//    private templateRef: TemplateRef<any>,
//    private viewContainer: ViewContainerRef,
//    private permissionService: PermissionService
//  ) { }
//}
