import { Component, OnInit, OnDestroy } from '@angular/core';
import { RouterModule } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { RoleService } from '../../services/role/role.service';
import { ModuleService } from '../../services/module/module.service';
import { AuthService } from '../../services/auth/auth.service';
import { MessageService, Message } from '../../services/general/message.service';
import { Role, Module, Permission, PermissionType, ModulePermission } from '../../models/role.model';
import { Subscription } from 'rxjs';
import { PaginationComponent } from '../../shared/pagination/pagination.component';
import { PagedResponse, PagedRequest } from '../../models/pagination.model';
import { PermissionDirective } from '../../directives/permission.directive';
import { FormsModule } from '@angular/forms';
import { DateRangeFilterComponent } from '../../shared/date-range-filter/date-range-filter.component';
import { DateFilterService, DateRange } from '../../services/general/date-filter.service';
import { ListService } from '../../services/general/list.service';
import { AuthorizationService } from '../../services/authorization/authorization.service';

@Component({
  selector: 'app-role-management',
  templateUrl: './role-management.component.html',
  styleUrls: ['./role-management.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    PaginationComponent,
    PermissionDirective,
    RouterModule,
    DateRangeFilterComponent
  ]
})
export class RoleManagementComponent implements OnInit, OnDestroy {
  roles: Role[] = [];
  modules: Module[] = [];
  roleForm!: FormGroup;
  isEditMode: boolean = false;
  currentRoleId: string | null = null;
  loading: boolean = false;
  message: Message | null = null;
  PermissionType = PermissionType;
  modulePermissions: Map<string, Map<PermissionType, boolean>> = new Map();
  Math = Math;

  // Filter properties
  selectedStatusFilter: string = 'all';

  // Pagination properties
  pagedResponse: PagedResponse<Role> | null = null;
  pageRequest: PagedRequest = {
    pageNumber: 1,
    pageSize: 10,
    searchText: '',
    sortColumn: 'name',
    sortDirection: 'asc'
  };

  private subscriptions: Subscription[] = [];
  private searchSubscription!: Subscription;
  private dateRangeSubscription!: Subscription;

  constructor(
    private roleService: RoleService,
    private moduleService: ModuleService,
    private authService: AuthService,
    private messageService: MessageService,
    private dateFilterService: DateFilterService,
    private listService: ListService,
    public authorizationService: AuthorizationService,
    private fb: FormBuilder
  ) { }

  ngOnInit(): void {
    this.initForm();
    this.loadRoles();
    this.loadModules();

    const messageSub = this.messageService.currentMessage.subscribe(message => {
      this.message = message;
    });

    this.subscriptions.push(messageSub);

    // Subscribe to search changes with debounce
    this.searchSubscription = this.listService.getSearchObservable().subscribe(term => {
      this.pageRequest.searchText = term;
      this.pageRequest.pageNumber = 1; // Reset to first page when search changes
      this.loadRoles();
    });

    // Subscribe to date range changes
    this.dateRangeSubscription = this.dateFilterService.getDateRangeObservable()
      .subscribe((dateRange: DateRange) => {
        this.pageRequest.startDate = dateRange.startDate ? dateRange.startDate.toISOString() : null;
        this.pageRequest.endDate = dateRange.endDate ? dateRange.endDate.toISOString() : null;
        this.pageRequest.pageNumber = 1; // Reset to first page when date filter changes
        this.loadRoles();
      });
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(sub => sub.unsubscribe());

    if (this.searchSubscription) {
      this.searchSubscription.unsubscribe();
    }

    if (this.dateRangeSubscription) {
      this.dateRangeSubscription.unsubscribe();
    }
  }

  private initForm(): void {
    this.roleForm = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(100)]],
      description: ['', [Validators.maxLength(500)]],
      isActive: [true]
    });
  }

  loadRoles(): void {
    this.loading = true;

    // Add status filter to request
    const isActive = this.selectedStatusFilter === 'all' ? undefined :
      this.selectedStatusFilter === 'active' ? true : false;

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
    this.loading = true;
    // Use the regular getModules instead of getAllModulesWithPermissions
    const sub = this.moduleService.getModules().subscribe({
      next: (modules) => {
        this.modules = modules.filter(m => m.isActive); // Only show active modules
        this.initializeModulePermissions();
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading modules:', error);
        this.messageService.showMessage({
          type: 'error',
          text: error.error?.message || 'Failed to load modules'
        });
        this.loading = false;
      }
    });
    this.subscriptions.push(sub);
  }

  initializeModulePermissions(): void {
    this.modulePermissions.clear();
    this.modules.forEach(module => {
      if (!module.id) return;

      const permissionMap = new Map<PermissionType, boolean>();
      permissionMap.set(PermissionType.View, false);
      //permissionMap.set(PermissionType.Add, false);
      //permissionMap.set(PermissionType.Edit, false);
      permissionMap.set(PermissionType.AddEdit, false);
      permissionMap.set(PermissionType.Delete, false);

      this.modulePermissions.set(module.id, permissionMap);
    });
  }

  togglePermission(moduleId: string, permType: PermissionType): void {
    const modulePerms = this.modulePermissions.get(moduleId);
    if (modulePerms) {
      const currentValue = modulePerms.get(permType) || false;

      // Toggle the selected permission
      modulePerms.set(permType, !currentValue);

      // If turning ON Edit or Delete permissions, automatically turn ON View permission
      if (!currentValue && (permType === PermissionType.AddEdit || permType === PermissionType.Delete)) {
        modulePerms.set(PermissionType.View, true);
      }
      if (!currentValue && permType === PermissionType.Delete) {
        modulePerms.set(PermissionType.View, true);
      }
    }
  }
  toggleAllPermissionsForModule(moduleId: string, checked: boolean): void {
    const modulePerms = this.modulePermissions.get(moduleId);
    if (modulePerms) {
      modulePerms.set(PermissionType.View, checked);
      //modulePerms.set(PermissionType.Add, checked);
      //modulePerms.set(PermissionType.Edit, checked);
      modulePerms.set(PermissionType.AddEdit, checked); 
      modulePerms.set(PermissionType.Delete, checked);
    }
  }

  isAllSelected(moduleId: string): boolean {
    const modulePerms = this.modulePermissions.get(moduleId);
    if (!modulePerms) return false;
    return (
      (modulePerms.get(PermissionType.View) || false) &&
      //(modulePerms.get(PermissionType.Add) || false) &&
      //(modulePerms.get(PermissionType.Edit) || false) &&
      (modulePerms.get(PermissionType.AddEdit) || false) &&
      (modulePerms.get(PermissionType.Delete) || false)
    );
  }

  handleCheckboxChange(moduleId: string, event: Event): void {
    const checkbox = event.target as HTMLInputElement;
    this.toggleAllPermissionsForModule(moduleId, checkbox.checked);
  }

  onSubmit(): void {
    if (this.roleForm.invalid) {
      Object.keys(this.roleForm.controls).forEach(key => {
        this.roleForm.get(key)?.markAsTouched();
      });
      return;
    }

    this.loading = true;
    const roleData = this.roleForm.value;

    // Convert module permissions to the backend format
    const permissions: any[] = [];

    this.modulePermissions.forEach((permMap, moduleId) => {
      const module = this.modules.find(m => m.id === moduleId);
      if (module) {
        permissions.push({
          moduleId: moduleId,
          moduleName: module.name,
          canView: permMap.get(PermissionType.View) || false,
          //canAdd: permMap.get(PermissionType.Add) || false,
          //canEdit: permMap.get(PermissionType.Edit) || false,
          canAddEdit: permMap.get(PermissionType.AddEdit) || false,
          canDelete: permMap.get(PermissionType.Delete) || false
        });
      }
    });

    roleData.permissions = permissions;

    if (this.isEditMode && this.currentRoleId) {
      const sub = this.roleService.updateRole(this.currentRoleId, roleData).subscribe({
        next: () => {
          this.handleSuccess('Role updated successfully');
        },
        error: (error) => {
          this.handleError('Failed to update role', error);
        }
      });
      this.subscriptions.push(sub);
    } else {
      const sub = this.roleService.createRole(roleData).subscribe({
        next: () => {
          this.handleSuccess('Role created successfully');
        },
        error: (error) => {
          this.handleError('Failed to create role', error);
        }
      });
      this.subscriptions.push(sub);
    }
  }

  private handleSuccess(message: string): void {
    this.messageService.showMessage({ type: 'success', text: message });
    this.resetForm();
    this.loadRoles();
    this.loading = false;
  }

  private handleError(message: string, error: any): void {
    console.error(message, error);
    this.messageService.showMessage({
      type: 'error',
      text: error.error?.message || message
    });
    this.loading = false;
  }

  editRole(role: Role): void {
    this.isEditMode = true;
    this.currentRoleId = role.id || null;

    // First get the full role details with permissions
    if (role.id) {
      this.loading = true;
      const sub = this.roleService.getRole(role.id).subscribe({
        next: (fullRole) => {
          this.roleForm.patchValue({
            name: fullRole.name,
            description: fullRole.description,
            isActive: fullRole.isActive
          });

          // Reset all permissions first
          this.initializeModulePermissions();

          // Set permissions from the role
          if (fullRole.permissions && Array.isArray(fullRole.permissions)) {
            fullRole.permissions.forEach((perm: any) => {
              if (perm.moduleId && this.modulePermissions.has(perm.moduleId)) {
                const modulePerms = this.modulePermissions.get(perm.moduleId);
                if (modulePerms) {
                  modulePerms.set(PermissionType.View, perm.canView || false);
                  //modulePerms.set(PermissionType.Add, perm.canAdd || false);
                  //modulePerms.set(PermissionType.Edit, perm.canEdit || false);
                  modulePerms.set(PermissionType.AddEdit, perm.canAddEdit || false);
                  modulePerms.set(PermissionType.Delete, perm.canDelete || false);
                }
              }
            });
          }

          this.loading = false;
          this.messageService.scrollToTop();
        },
        error: (error) => {
          console.error('Error loading role details:', error);
          this.messageService.showMessage({
            type: 'error',
            text: 'Failed to load role details'
          });
          this.loading = false;
        }
      });
      this.subscriptions.push(sub);
    }
  }

  deleteRole(id: string): void {
    if (confirm('Are you sure you want to delete this role?')) {
      this.loading = true;
      this.roleService.deleteRole(id).subscribe({
        next: () => this.handleSuccess('Role deleted successfully'),
        error: (error) => this.handleError('Failed to delete role', error)
      });
    }
  }

  resetForm(): void {
    this.roleForm.reset({ isActive: true });
    this.isEditMode = false;
    this.currentRoleId = null;
    this.initializeModulePermissions();
  }

  onPageChange(newPage: number): void {
    this.pageRequest.pageNumber = newPage;
    this.loadRoles();
  }

  onSearchChange(event: Event): void {
    const target = event.target as HTMLInputElement;
    this.listService.search(target.value);
  }

  onPageSizeChange(event: Event): void {
    const selectElement = event.target as HTMLSelectElement;
    this.pageRequest.pageSize = Number(selectElement.value);
    this.pageRequest.pageNumber = 1; // Reset to first page when changing page size
    this.loadRoles();
  }

  onSortChange(column: string): void {
    if (this.pageRequest.sortColumn === column) {
      // Toggle direction if same column is clicked
      this.pageRequest.sortDirection = this.pageRequest.sortDirection === 'asc' ? 'desc' : 'asc';
    } else {
      // Set new column and default to ascending
      this.pageRequest.sortColumn = column;
      this.pageRequest.sortDirection = 'asc';
    }
    this.loadRoles();
  }

  onStatusFilterChange(event: Event): void {
    const selectElement = event.target as HTMLSelectElement;
    this.selectedStatusFilter = selectElement.value;
    this.pageRequest.pageNumber = 1; // Reset to first page when filter changes
    this.loadRoles();
  }
}
