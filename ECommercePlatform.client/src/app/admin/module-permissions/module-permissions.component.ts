import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { ModuleService } from '../../services/module/module.service';
import { AuthorizationService } from '../../services/authorization/authorization.service';
import { MessageService, Message } from '../../services/general/message.service';
import { Module, Permission, PermissionType } from '../../models/role.model';
import { Subscription } from 'rxjs';
import { PermissionDirective } from '../../directives/permission.directive';

@Component({
  selector: 'app-module-permissions',
  templateUrl: './module-permissions.component.html',
  styleUrls: ['./module-permissions.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    PermissionDirective,
    RouterModule
  ]
})
export class ModulePermissionsComponent implements OnInit, OnDestroy {
  module: Module | null = null;
  permissions: Permission[] = [];
  permissionForm!: FormGroup;
  isEditMode: boolean = false;
  currentPermissionId: string | null = null;
  loading: boolean = false;
  message: Message | null = null;
  PermissionType = PermissionType;
  permissionTypes = Object.values(PermissionType);

  private moduleId: string | null = null;
  private subscriptions: Subscription[] = [];

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private moduleService: ModuleService,
    private authorizationService: AuthorizationService,
    private messageService: MessageService,
    private fb: FormBuilder
  ) { }

  ngOnInit(): void {
    this.initForm();

    // Get module ID from route parameters
    const routeSub = this.route.paramMap.subscribe(params => {
      this.moduleId = params.get('id');
      if (this.moduleId) {
        this.loadModule(this.moduleId);
      } else {
        this.router.navigate(['/admin/modules']);
      }
    });

    const messageSub = this.messageService.currentMessage.subscribe(message => {
      this.message = message;
    });

    this.subscriptions.push(routeSub, messageSub);
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  private initForm(): void {
    this.permissionForm = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(100)]],
      description: ['', [Validators.maxLength(250)]],
      type: [PermissionType.VIEW, [Validators.required]],
      isActive: [true]
    });
  }

  loadModule(id: string): void {
    this.loading = true;
    const sub = this.moduleService.getModuleWithPermissions(id).subscribe({
      next: (module) => {
        this.module = module;
        this.permissions = module.permissions || [];
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading module permissions:', error);
        this.messageService.showMessage({
          type: 'error',
          text: error.error?.message || 'Failed to load module permissions'
        });
        this.loading = false;
        this.router.navigate(['/admin/modules']);
      }
    });

    this.subscriptions.push(sub);
  }

  onSubmit(): void {
    if (this.permissionForm.invalid || !this.moduleId) {
      // Mark all fields as touched to show validation errors
      Object.keys(this.permissionForm.controls).forEach(key => {
        this.permissionForm.get(key)?.markAsTouched();
      });
      return;
    }

    const permissionData: any = {
      ...this.permissionForm.value,
      moduleId: this.moduleId
    };

    this.loading = true;

    if (this.isEditMode && this.currentPermissionId) {
      // This would be the updatePermission method if implemented
      this.loading = false;
      this.messageService.showMessage({
        type: 'info',
        text: 'Permission update functionality is not yet implemented'
      });
    } else {
      // This would be the createPermission method if implemented
      this.loading = false;
      this.messageService.showMessage({
        type: 'info',
        text: 'Permission creation functionality is not yet implemented'
      });
    }

    // Mock implementation for UI demonstration
    setTimeout(() => {
      if (!this.isEditMode) {
        // Simulate permission creation
        const newPermission: Permission = {
          id: crypto.randomUUID(),
          name: permissionData.name,
          description: permissionData.description,
          type: permissionData.type,
          moduleId: this.moduleId as string,
          moduleName: this.module?.name,
          isActive: permissionData.isActive
        };
        this.permissions = [...this.permissions, newPermission];
      } else {
        // Simulate permission update
        this.permissions = this.permissions.map(p =>
          p.id === this.currentPermissionId
            ? { ...p, ...permissionData }
            : p
        );
      }

      // Reset form and state
      this.resetForm();
      this.messageService.showMessage({
        type: 'success',
        text: `Permission ${this.isEditMode ? 'updated' : 'created'} successfully`
      });
      this.loading = false;

      // Clear authorization cache to refresh permissions
      this.authorizationService.clearCache();
    }, 500);
  }

  editPermission(permission: Permission): void {
    this.isEditMode = true;
    this.currentPermissionId = permission.id || null;

    this.permissionForm.patchValue({
      name: permission.name,
      description: permission.description,
      type: permission.type,
      isActive: permission.isActive
    });

    this.messageService.scrollToTop();
  }

  deletePermission(id: string | undefined): void {
    if (!id) return;

    if (confirm('Are you sure you want to delete this permission? This may affect user roles that use this permission.')) {
      this.loading = true;

      // Mock implementation for UI demonstration
      setTimeout(() => {
        this.permissions = this.permissions.filter(p => p.id !== id);
        this.messageService.showMessage({
          type: 'success',
          text: 'Permission deleted successfully'
        });
        this.loading = false;

        // Clear authorization cache to refresh permissions
        this.authorizationService.clearCache();
      }, 500);
    }
  }

  resetForm(): void {
    this.permissionForm.reset({
      type: PermissionType.VIEW,
      isActive: true
    });
    this.isEditMode = false;
    this.currentPermissionId = null;
  }

  backToModules(): void {
    this.router.navigate(['/admin/modules']);
  }

  getPermissionTypeClass(type: PermissionType | undefined): string {
    if (!type) return 'badge bg-info';

    switch (type) {
      case PermissionType.VIEW: return 'badge bg-primary';
      case PermissionType.ADD: return 'badge bg-success';
      case PermissionType.EDIT: return 'badge bg-warning';
      case PermissionType.DELETE: return 'badge bg-danger';
      default: return 'badge bg-info';
    }
  }
}
