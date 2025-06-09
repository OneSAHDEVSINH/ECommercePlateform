import { Component, OnInit, OnDestroy } from '@angular/core';
import { RouterModule } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { RoleService } from '../../services/role/role.service';
import { AuthService } from '../../services/auth/auth.service';
import { MessageService, Message } from '../../services/general/message.service';
import { Role, Module, Permission, PermissionType, ModulePermission } from '../../models/role.model';
import { Subscription } from 'rxjs';
import { PaginationComponent } from '../../shared/pagination/pagination.component';
import { PagedResponse, PagedRequest } from '../../models/pagination.model';
import { PermissionDirective } from '../../directives/permission.directive';

@Component({
  selector: 'app-role-management',
  templateUrl: './role-management.component.html',
  styleUrls: ['./role-management.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    PaginationComponent,
    //PermissionDirective,
    RouterModule
  ]
})
export class RoleManagementComponent implements OnInit, OnDestroy {
  roles: Role[] = [];
  modules: Module[] = [];
  roleForm!: FormGroup;
  permissionsForm!: FormGroup;
  isEditMode: boolean = false;
  currentRoleId: string | null = null;
  loading: boolean = false;
  message: Message | null = null;
  //permissionTypes = Object.values(PermissionType);
  PermissionType = PermissionType;
  permissionTypes = [PermissionType.VIEW, PermissionType.ADD, PermissionType.EDIT, PermissionType.DELETE];
  selectedPermissions: { moduleId: string, permissionType: string }[] = [];
  Math = Math;

  // Pagination properties
  pagedResponse: PagedResponse<Role> | null = null;
  pageRequest: PagedRequest = {
    pageNumber: 1,
    pageSize: 10,
    searchText: '',
    sortColumn: 'name',
    sortDirection: 'asc'
  };

  private currentUser: any = null;
  private subscriptions: Subscription[] = [];

  constructor(
    private roleService: RoleService,
    private authService: AuthService,
    private messageService: MessageService,
    private fb: FormBuilder
  ) { }

  ngOnInit(): void {
    this.initForms();
    this.loadRoles();
    this.loadModules();

    const userSub = this.authService.currentUser$.subscribe(user => {
      this.currentUser = user;
    });

    const messageSub = this.messageService.currentMessage.subscribe(message => {
      this.message = message;
    });

    this.subscriptions.push(userSub, messageSub);
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  private initForms(): void {
    // Basic role information form
    this.roleForm = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(100)]],
      description: ['', [Validators.maxLength(500)]],
      isActive: [true]
    });
  }

  loadRoles(): void {
    this.loading = true;
    const sub = this.roleService.getPagedRoles(this.pageRequest).subscribe({
      next: (response) => {
        this.pagedResponse = response;
        this.roles = response.items;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading roles:', error);
        this.messageService.showMessage({
          type: 'error',
          text: error.error?.message || 'Failed to load roles'
        });
        this.loading = false;
      }
    });

    this.subscriptions.push(sub);
  }

  loadModules(): void {
    const sub = this.roleService.getModules().subscribe({
      next: (modules) => {
        this.modules = modules;
      },
      error: (error) => {
        console.error('Error loading modules:', error);
      }
    });

    this.subscriptions.push(sub);
  }

  isChecked(moduleId: string, perm: string): boolean {
    return this.selectedPermissions.some(p => p.moduleId === moduleId && p.permissionType === perm);
  }

  onPermissionChange(moduleId: string, perm: string, event: Event) {
    const checked = (event.target as HTMLInputElement).checked;
    if (checked) {
      this.selectedPermissions.push({ moduleId, permissionType: perm });
    } else {
      this.selectedPermissions = this.selectedPermissions.filter(
        p => !(p.moduleId === moduleId && p.permissionType === perm)
      );
    }
  }

  onSubmit(): void {
    if (this.roleForm.invalid) {
      return;
    }

    const roleData: Role = {
      ...this.roleForm.value,
      id: this.isEditMode && this.currentRoleId ? this.currentRoleId : undefined,
      createdBy: this.isEditMode ? undefined : this.getUserIdentifier(),
      modifiedBy: this.getUserIdentifier()
    };

    this.loading = true;

    if (this.isEditMode && this.currentRoleId) {
      const sub = this.roleService.updateRole(this.currentRoleId, roleData).subscribe({
        next: () => {
          this.messageService.showMessage({ type: 'success', text: 'Role updated successfully' });
          this.loadRoles();
          this.resetForm();
          this.loading = false;
        },
        error: (error) => {
          console.error('Error updating role:', error);
          this.messageService.showMessage({
            type: 'error',
            text: error.error?.message || 'Failed to update role'
          });
          this.loading = false;
        }
      });

      this.subscriptions.push(sub);
    } else {
      const sub = this.roleService.createRole(roleData).subscribe({
        next: (newRole) => {
          this.messageService.showMessage({ type: 'success', text: 'Role created successfully' });
          this.loadRoles();
          this.resetForm();
          this.loading = false;
        },
        error: (error) => {
          console.error('Error creating role:', error);
          this.messageService.showMessage({
            type: 'error',
            text: error.error?.message || 'Failed to create role'
          });
          this.loading = false;
        }
      });

      this.subscriptions.push(sub);
    }
  }

  editRole(role: Role): void {
    this.isEditMode = true;
    this.currentRoleId = role.id || null;

    this.roleForm.patchValue({
      name: role.name,
      description: role.description,
      isActive: role.isActive
    });

    //// Load role permissions if editing
    //if (role.id) {
    //  this.loadRolePermissions(role.id);
    //}

    this.selectedPermissions = [];
    if (role.permissions) {
      role.permissions.forEach((perm: any) => {
        this.selectedPermissions.push({ moduleId: perm.moduleId, permissionType: perm.permissionType });
      });
    }

    this.messageService.scrollToTop();
  }

  loadRolePermissions(roleId: string): void {
    const sub = this.roleService.getRolePermissions(roleId).subscribe({
      next: (permissions) => {
        // Set permissions in the form
        permissions.forEach(modulePermission => {
          // Update UI to show current permissions
        });
      },
      error: (error) => {
        console.error('Error loading role permissions:', error);
      }
    });

    this.subscriptions.push(sub);
  }

  savePermissions(): void {
    if (!this.currentRoleId) return;

    // Gather selected permissions
    const permissionRequest = {
      roleId: this.currentRoleId,
      permissions: this.modules.map(module => {
        return {
          moduleId: module.id,
          permissionTypes: this.getSelectedPermissions(module.id)
        };
      }).filter(p => p.permissionTypes.length > 0)
    };

    this.loading = true;

    const sub = this.roleService.assignPermissionsToRole(permissionRequest).subscribe({
      next: () => {
        this.messageService.showMessage({
          type: 'success',
          text: 'Permissions saved successfully'
        });
        this.loading = false;
      },
      error: (error) => {
        console.error('Error saving permissions:', error);
        this.messageService.showMessage({
          type: 'error',
          text: error.error?.message || 'Failed to save permissions'
        });
        this.loading = false;
      }
    });

    this.subscriptions.push(sub);
  }

  selectAllPermissions(event: Event, moduleId: string): void {
    const isChecked = (event.target as HTMLInputElement).checked;

    // Find all checkboxes for this module and update their state
    const viewCheckbox = document.getElementById(`view-${moduleId}`) as HTMLInputElement;
    const addEditCheckbox = document.getElementById(`addEdit-${moduleId}`) as HTMLInputElement;
    const deleteCheckbox = document.getElementById(`delete-${moduleId}`) as HTMLInputElement;

    if (viewCheckbox) viewCheckbox.checked = isChecked;
    if (addEditCheckbox) addEditCheckbox.checked = isChecked;
    if (deleteCheckbox) deleteCheckbox.checked = isChecked;

    // Update the selected permissions in the component's data model
    // This would need to be updated to properly reflect your permissions data structure
    const modulePermissions = this.getSelectedPermissions(moduleId);

    // Reset and add all permissions based on checked state
    if (isChecked) {
      modulePermissions.push(
        PermissionType.VIEW,
        PermissionType.ADD,
        PermissionType.DELETE
      );
    }
  }

  getSelectedPermissions(moduleId: string): PermissionType[] {
    const selectedTypes: PermissionType[] = [];

    const viewCheckbox = document.getElementById(`view-${moduleId}`) as HTMLInputElement;
    const addEditCheckbox = document.getElementById(`addEdit-${moduleId}`) as HTMLInputElement;
    const deleteCheckbox = document.getElementById(`delete-${moduleId}`) as HTMLInputElement;

    if (viewCheckbox?.checked) {
      selectedTypes.push(PermissionType.VIEW);
    }

    if (addEditCheckbox?.checked) {
      selectedTypes.push(PermissionType.ADD);
    }

    if (deleteCheckbox?.checked) {
      selectedTypes.push(PermissionType.DELETE);
    }

    return selectedTypes;
  }

  deleteRole(id: string | undefined): void {
    if (!id) return;

    if (confirm('Are you sure you want to delete this role?')) {
      this.loading = true;
      const sub = this.roleService.deleteRole(id).subscribe({
        next: () => {
          this.messageService.showMessage({ type: 'success', text: 'Role deleted successfully' });
          this.loadRoles();
          this.loading = false;
        },
        error: (error) => {
          console.error('Error deleting role:', error);
          this.messageService.showMessage({
            type: 'error',
            text: error.error?.message || 'Failed to delete role'
          });
          this.loading = false;
        }
      });

      this.subscriptions.push(sub);
    }
  }

  resetForm(): void {
    this.roleForm.reset({
      isActive: true
    });
    this.isEditMode = false;
    this.currentRoleId = null;
    this.selectedPermissions = [];
  }

  private getUserIdentifier(): string {
    return this.currentUser ? this.currentUser.id || this.currentUser.email : 'system';
  }

  // Helper methods to replace arrow functions in the template
  areAllPermissionsChecked(moduleId: string): boolean {
    return this.permissionTypes.every(perm => this.isChecked(moduleId, perm));
  }

  toggleAllPermissions(moduleId: string, event: Event): void {
    const checked = (event.target as HTMLInputElement).checked;
    this.permissionTypes.forEach(perm => this.onPermissionChange(moduleId, perm, event));
  }

  // Pagination methods
  onPageChange(page: number | Event): void {
    // Handle both number and Event types
    const pageNumber = typeof page === 'number' ? page : 1;
    this.pageRequest.pageNumber = pageNumber;
    this.loadRoles();
  }

  onPageSizeChange(event: Event): void {
    const selectElement = event.target as HTMLSelectElement;
    this.pageRequest.pageSize = +selectElement.value;
    this.pageRequest.pageNumber = 1;
    this.loadRoles();
  }

  onSortChange(column: string): void {
    if (this.pageRequest.sortColumn === column) {
      this.pageRequest.sortDirection =
        this.pageRequest.sortDirection === 'asc' ? 'desc' : 'asc';
    } else {
      this.pageRequest.sortColumn = column;
      this.pageRequest.sortDirection = 'asc';
    }
    this.loadRoles();
  }

  onSearchChange(event: Event): void {
    const searchTerm = (event.target as HTMLInputElement).value;
    this.pageRequest.searchText = searchTerm;
    this.pageRequest.pageNumber = 1;
    this.loadRoles();
  }
}
