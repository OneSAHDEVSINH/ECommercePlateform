import { Component, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { RouterModule } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ModuleService } from '../../services/module/module.service';
import { AuthorizationService } from '../../services/authorization/authorization.service';
import { MessageService, Message } from '../../services/general/message.service';
import { Module, PermissionType } from '../../models/role.model';
import { Subscription } from 'rxjs';
import { PaginationComponent } from '../../shared/pagination/pagination.component';
import { PermissionDirective } from '../../directives/permission.directive';
import { PagedResponse, PagedRequest } from '../../models/pagination.model';
import { FormsModule } from '@angular/forms';
import { DateRangeFilterComponent } from '../../shared/date-range-filter/date-range-filter.component';
import { DateFilterService, DateRange } from '../../services/general/date-filter.service';
import { ListService } from '../../services/general/list.service';

@Component({
  selector: 'app-module',
  templateUrl: './module.component.html',
  styleUrls: ['./module.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    PaginationComponent,
    RouterModule,
    DateRangeFilterComponent
  ]
})
export class ModuleComponent implements OnInit, OnDestroy {
  modules: Module[] = [];
  moduleForm!: FormGroup;
  isEditMode: boolean = false;
  currentModuleId: string | null = null;
  loading: boolean = false;
  message: Message | null = null;
  PermissionType = PermissionType;
  Math = Math;
  private subscriptions: Subscription[] = [];
  private searchSubscription!: Subscription;
  private dateRangeSubscription!: Subscription;
  private permissionSubscription!: Subscription;

  // Permission-dependent UI state
  canAddEdit: boolean = false;
  canDelete: boolean = false;
  canView: boolean = false;

  // Filter properties
  selectedStatusFilter: string = 'all';

  // Pagination properties
  pagedResponse: PagedResponse<Module> | null = null;
  pageRequest: PagedRequest = {
    pageNumber: 1,
    pageSize: 10,
    searchText: '',
    sortColumn: 'displayOrder',
    sortDirection: 'asc'
  };

  constructor(
    private moduleService: ModuleService,
    public authorizationService: AuthorizationService,
    private messageService: MessageService,
    private dateFilterService: DateFilterService,
    private listService: ListService,
    private fb: FormBuilder,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.initForm();
    this.loadModules();

    // Initial permission check
    this.checkPermissions();

    // Subscribe to global permission changes
    this.permissionSubscription = this.authorizationService.globalPermissionChange$.subscribe(() => {
      this.checkPermissions(); // Refresh permission-dependent state
      this.cdr.detectChanges(); // Force change detection
    });

    const messageSub = this.messageService.currentMessage.subscribe(message => {
      this.message = message;
    });

    // Clear authorization cache when managing modules to ensure permissions are up-to-date
    this.authorizationService.clearCache();

    this.subscriptions.push(messageSub);

    // Subscribe to search changes with debounce
    this.searchSubscription = this.listService.getSearchObservable().subscribe(term => {
      this.pageRequest.searchText = term;
      this.pageRequest.pageNumber = 1; // Reset to first page when search changes
      this.loadModules();
    });

    // Subscribe to date range changes
    this.dateRangeSubscription = this.dateFilterService.getDateRangeObservable()
      .subscribe((dateRange: DateRange) => {
        this.pageRequest.startDate = dateRange.startDate ? dateRange.startDate.toISOString() : null;
        this.pageRequest.endDate = dateRange.endDate ? dateRange.endDate.toISOString() : null;
        this.pageRequest.pageNumber = 1; // Reset to first page when date filter changes
        this.loadModules();
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
    if (this.permissionSubscription) {
      this.permissionSubscription.unsubscribe();
    }
  }

  // method to check permissions
  private checkPermissions(): void {
    this.canView = this.authorizationService.hasPermission('modules', PermissionType.View);
    this.canAddEdit = this.authorizationService.hasPermission('modules', PermissionType.AddEdit);
    this.canDelete = this.authorizationService.hasPermission('modules', PermissionType.Delete);

    console.log('Modules permissions updated:', { canView: this.canView, canAddEdit: this.canAddEdit, canDelete: this.canDelete });
  }

  private initForm(): void {
    this.moduleForm = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(100)]],
      description: ['', [Validators.maxLength(500)]],
      route: ['', [Validators.required, Validators.maxLength(50), Validators.pattern('^[a-z0-9-]+$')]],
      icon: ['', [Validators.maxLength(50)]],
      displayOrder: [0, [Validators.required, Validators.min(0)]],
      isActive: [true]
    });
  }

  loadModules(): void {
    this.loading = true;

    // Add status filter to request
    const isActive = this.selectedStatusFilter === 'all' ? undefined :
      this.selectedStatusFilter === 'active' ? true : false;

    const sub = this.moduleService.getPagedModules(this.pageRequest, false).subscribe({
      next: (response) => {
        this.pagedResponse = response;
        this.modules = response.items;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading modules:', error);
        const errorMessage = error.error?.message ||
          error.error?.title ||
          error.message ||
          'Failed to load modules';
        this.messageService.showMessage({ type: 'error', text: errorMessage });
        this.loading = false;
      }
    });

    this.subscriptions.push(sub);
  }

  onSubmit(): void {
    if (this.moduleForm.invalid) {
      // Mark all fields as touched to trigger validation messages
      Object.keys(this.moduleForm.controls).forEach(key => {
        const control = this.moduleForm.get(key);
        control?.markAsTouched();
      });
      return;
    }

    this.loading = true;
    const moduleData: Module = this.moduleForm.value;

    if (this.isEditMode && this.currentModuleId) {
      const sub = this.moduleService.updateModule(this.currentModuleId, moduleData).subscribe({
        next: () => {
          this.messageService.showMessage({
            type: 'success',
            text: 'Module updated successfully'
          });
          this.loadModules();
          this.resetForm();
          this.loading = false;
          // Clear authorization cache after updating a module
          this.authorizationService.clearCache();
        },
        error: (error) => {
          console.error('Error updating module:', error);
          this.messageService.showMessage({
            type: 'error',
            text: error.error?.message || 'Failed to update module'
          });
          this.loading = false;
        }
      });

      this.subscriptions.push(sub);
    } else {
      const sub = this.moduleService.createModule(moduleData).subscribe({
        next: () => {
          this.messageService.showMessage({
            type: 'success',
            text: 'Module created successfully'
          });
          this.loadModules();
          this.resetForm();
          this.loading = false;
          // Clear authorization cache after creating a module
          this.authorizationService.clearCache();
        },
        error: (error) => {
          console.error('Error creating module:', error);
          this.messageService.showMessage({
            type: 'error',
            text: error.error?.message || 'Failed to create module'
          });
          this.loading = false;
        }
      });

      this.subscriptions.push(sub);
    }
  }

  editModule(module: Module): void {
    this.isEditMode = true;
    this.currentModuleId = module.id || null;

    this.moduleForm.patchValue({
      name: module.name,
      description: module.description,
      route: module.route,
      icon: module.icon,
      displayOrder: module.displayOrder,
      isActive: module.isActive
    });

    this.messageService.scrollToTop();
  }

  deleteModule(id: string | undefined): void {
    if (!id) return;

    if (confirm('Are you sure you want to delete this module? This will also delete all permissions associated with this module and may affect user roles.')) {
      this.loading = true;
      const sub = this.moduleService.deleteModule(id).subscribe({
        next: () => {
          this.messageService.showMessage({
            type: 'success',
            text: 'Module deleted successfully'
          });
          this.loadModules();
          this.loading = false;
          // Clear authorization cache after deleting a module
          this.authorizationService.clearCache();
        },
        error: (error) => {
          console.error('Error deleting module:', error);
          this.messageService.showMessage({
            type: 'error',
            text: error.error?.message || 'Failed to delete module'
          });
          this.loading = false;
        }
      });

      this.subscriptions.push(sub);
    }
  }

  resetForm(): void {
    this.moduleForm.reset({
      displayOrder: 0,
      isActive: true
    });
    this.isEditMode = false;
    this.currentModuleId = null;
  }

  toggleStatus(module: Module): void {
    if (!module.id || !this.authorizationService.hasPermission('modules', PermissionType.AddEdit)) return;

    this.loading = true;

    // Create a simple update object with just the toggled status
    const update = {
      name: module.name,
      description: module.description,
      route: module.route,
      icon: module.icon,
      displayOrder: module.displayOrder,
      isActive: !module.isActive
    };

    this.moduleService.updateModule(module.id, update).subscribe({
      next: () => {
        // Update the item in the local array to avoid a full reload
        module.isActive = !module.isActive;
        this.messageService.showMessage({
          type: 'success',
          text: `Module ${module.isActive ? 'activated' : 'deactivated'} successfully`
        });
        this.loadModules();
        this.loading = false;
      },
      error: (error) => {
        console.error('Error updating status:', error);
        const errorMessage = error.error?.message || 'Failed to update status';
        this.messageService.showMessage({ type: 'error', text: errorMessage });
        this.loading = false;
      }
    });
  }

  // Pagination methods
  onPageChange(page: number): void {
    this.pageRequest.pageNumber = page;
    this.loadModules();
  }

  onPageSizeChange(event: Event): void {
    const selectElement = event.target as HTMLSelectElement;
    this.pageRequest.pageSize = Number(selectElement.value);
    this.pageRequest.pageNumber = 1;
    this.loadModules();
  }

  onSortChange(column: string): void {
    if (this.pageRequest.sortColumn === column) {
      this.pageRequest.sortDirection = this.pageRequest.sortDirection === 'asc' ? 'desc' : 'asc';
    } else {
      this.pageRequest.sortColumn = column;
      this.pageRequest.sortDirection = 'asc';
    }
    this.loadModules();
  }

  onSearchChange(event: Event): void {
    const target = event.target as HTMLInputElement;
    this.listService.search(target.value);
  }

  onStatusFilterChange(event: Event): void {
    const selectElement = event.target as HTMLSelectElement;
    this.selectedStatusFilter = selectElement.value;
    this.pageRequest.pageNumber = 1; // Reset to first page when filter changes
    this.loadModules();
  }
}
