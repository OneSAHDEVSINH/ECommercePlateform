import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { Subscription } from 'rxjs';
import { genderToString, stringToGender, User } from '../../models/user.model';
import { Role, PermissionType } from '../../models/role.model';
import { PagedResponse, PagedRequest } from '../../models/pagination.model';
import { UserService } from '../../services/user/user.service';
import { RoleService } from '../../services/role/role.service';
import { MessageService, Message } from '../../services/general/message.service';
import { DateFilterService, DateRange } from '../../services/general/date-filter.service';
import { ListService } from '../../services/general/list.service';
import { PaginationComponent } from '../../shared/pagination/pagination.component';
import { DateRangeFilterComponent } from '../../shared/date-range-filter/date-range-filter.component';
import { PermissionDirective } from '../../directives/permission.directive';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { AuthorizationService } from '../../services/authorization/authorization.service';

@Component({
  selector: 'app-user',
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    RouterModule,
    PaginationComponent,
    DateRangeFilterComponent,
    PermissionDirective
  ]
})
export class UserComponent implements OnInit, OnDestroy {
  users: User[] = [];
  availableRoles: Role[] = [];
  userForm!: FormGroup;
  isEditMode: boolean = false;
  currentUserId: string | null = null;
  loading: boolean = false;
  rolesLoading: boolean = false;
  message: Message | null = null;

  // Selected roles for the user being edited/created
  selectedRoles: string[] = [];
  selectedRoleId: string = 'all';
  Math = Math;
  PermissionType = PermissionType;

  // Pagination properties
  pagedResponse: PagedResponse<User> | null = null;
  pageRequest: PagedRequest = {
    pageNumber: 1,
    pageSize: 10,
    searchText: '',
    sortColumn: 'firstName',
    sortDirection: 'asc'
  };

  // Subscriptions for clean up
  private messageSubscription!: Subscription;
  private searchSubscription!: Subscription;
  private dateRangeSubscription!: Subscription;

  constructor(
    private userService: UserService,
    private roleService: RoleService,
    private messageService: MessageService,
    private dateFilterService: DateFilterService,
    private listService: ListService,
    public authorizationService: AuthorizationService,
    private fb: FormBuilder
  ) { }

  ngOnInit(): void {
    this.initForm();
    this.loadUsers();
    this.loadRoles();

    // Subscribe to messages
    this.messageSubscription = this.messageService.currentMessage.subscribe(message => {
      this.message = message;
    });

    // Subscribe to search terms with debounce
    this.searchSubscription = this.listService.getSearchObservable().subscribe(term => {
      this.pageRequest.searchText = term;
      this.pageRequest.pageNumber = 1; // Reset to first page when search changes
      this.loadUsers();
    });

    // Subscribe to date range filter
    this.dateRangeSubscription = this.dateFilterService.getDateRangeObservable()
      .subscribe((dateRange: DateRange) => {
        this.pageRequest.startDate = dateRange.startDate ? dateRange.startDate.toISOString() : null;
        this.pageRequest.endDate = dateRange.endDate ? dateRange.endDate.toISOString() : null;
        this.pageRequest.pageNumber = 1;
        this.loadUsers();
      });
  }

  ngOnDestroy(): void {
    // Clean up subscriptions
    if (this.messageSubscription) {
      this.messageSubscription.unsubscribe();
    }
    if (this.searchSubscription) {
      this.searchSubscription.unsubscribe();
    }
    if (this.dateRangeSubscription) {
      this.dateRangeSubscription.unsubscribe();
    }
  }

  private initForm(): void {
    this.userForm = this.fb.group({
      firstName: ['', [Validators.required, Validators.maxLength(50)]],
      lastName: ['', [Validators.required, Validators.maxLength(50)]],
      email: ['', [Validators.required, Validators.email, Validators.maxLength(100)]],
      password: ['', this.isEditMode ? [] : [Validators.required, Validators.minLength(6), Validators.maxLength(100)]],
      phoneNumber: ['', [Validators.maxLength(20)]],
      gender: [''],
      dateOfBirth: [null],
      bio: ['', [Validators.maxLength(500)]],
      isActive: [true]
    });
  }

  loadUsers(): void {
    this.loading = true;

    // Create a copy of the request with additional parameters
    const request = {
      ...this.pageRequest,
      includeRoles: true
    };

    this.userService.getPagedUsers(
      request,
      this.selectedRoleId !== 'all' ? this.selectedRoleId : undefined, false
    ).subscribe({
      next: (response) => {
        this.pagedResponse = response;

        // Filter out ONLY the default SuperAdmin by email, not all SuperAdmin role users
        let filteredUsers = !this.authorizationService.isAdmin()
          ? response.items.filter(user => user.email?.toLowerCase() !== 'admin@admin.com')
          : response.items;

        // Convert gender enum values to string representation
        this.users = filteredUsers.map(user => ({
          ...user,
          gender: genderToString(user.gender)
        }));

        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading users:', error);
        const errorMessage = error.error?.message ||
          error.error?.title ||
          error.message ||
          'Failed to load users';
        this.messageService.showMessage({ type: 'error', text: errorMessage });
        this.loading = false;
      }
    });
  }

  loadRoles(): void {
    this.rolesLoading = true;
    this.roleService.getRoles().subscribe({
      next: (roles) => {
        this.availableRoles = roles;
        this.rolesLoading = false;
      },
      error: (error) => {
        console.error('Error loading roles:', error);
        this.messageService.showMessage({
          type: 'error',
          text: 'Failed to load roles. Role assignment may not work properly.'
        });
        this.rolesLoading = false;
      }
    });
  }

  onSubmit(): void {
    if (this.userForm.invalid) {
      // Mark all fields as touched to show validation errors
      Object.keys(this.userForm.controls).forEach(key => {
        this.userForm.get(key)?.markAsTouched();
      });
      return;
    }

    this.loading = true;

    // Prepare user data from form
    const userData: any = {
      ...this.userForm.value,
      roleIds: this.selectedRoles
    };

    // Handle empty password in edit mode
    if (this.isEditMode && !userData.password) {
      delete userData.password;
    }

    if (this.isEditMode && this.currentUserId) {
      userData.id = this.currentUserId;

      this.userService.updateUser(this.currentUserId, userData).subscribe({
        next: () => {
          this.messageService.showMessage({
            type: 'success',
            text: 'User updated successfully'
          });
          this.loadUsers();
          this.resetForm();
          this.loading = false;
        },
        error: (error) => {
          console.error('Error updating user:', error);
          const errorMessage = error.error?.message ||
            error.error?.title ||
            error.message ||
            'Failed to update user';
          this.messageService.showMessage({ type: 'error', text: errorMessage });
          this.loading = false;
        }
      });
    } else {
      this.userService.createUser(userData).subscribe({
        next: () => {
          this.messageService.showMessage({
            type: 'success',
            text: 'User created successfully'
          });
          this.loadUsers();
          this.resetForm();
          this.loading = false;
        },
        error: (error) => {
          console.error('Error creating user:', error);
          const errorMessage = error.error?.message ||
            error.error?.title ||
            error.message ||
            'Failed to create user';
          this.messageService.showMessage({ type: 'error', text: errorMessage });
          this.loading = false;
        }
      });
    }
  }

  editUser(user: User): void {
    this.isEditMode = true;
    this.currentUserId = user.id || null;

    if (this.currentUserId) {
      this.loading = true;

      // Fetch complete user details including roles
      this.userService.getUserById(this.currentUserId).subscribe({
        next: (userDetails) => {
          // Reset form and update validators for password in edit mode
          this.userForm.reset();
          const passwordControl = this.userForm.get('password');
          if (passwordControl) {
            passwordControl.clearValidators();
            passwordControl.updateValueAndValidity();
          }

          // Populate form with user data
          this.userForm.patchValue({
            firstName: userDetails.firstName,
            lastName: userDetails.lastName,
            email: userDetails.email,
            phoneNumber: userDetails.phoneNumber || '',
            gender: genderToString(userDetails.gender),
            dateOfBirth: userDetails.dateOfBirth ? new Date(userDetails.dateOfBirth).toISOString().split('T')[0] : null,
            bio: userDetails.bio || '',
            isActive: userDetails.isActive !== undefined ? userDetails.isActive : true
          });

          // Set selected roles
          this.selectedRoles = userDetails.roles?.map(role => role.id || '') || [];

          this.loading = false;
          // Scroll to top of form
          this.messageService.scrollToTop();
        },
        error: (error) => {
          console.error('Error fetching user details:', error);
          const errorMessage = error.error?.message ||
            error.error?.title ||
            error.message ||
            'Failed to fetch user details';
          this.messageService.showMessage({ type: 'error', text: errorMessage });
          this.loading = false;
        }
      });
    }
  }

  deleteUser(id: string): void {
    if (confirm('Are you sure you want to delete this user?')) {
      this.loading = true;
      this.userService.deleteUser(id).subscribe({
        next: () => {
          this.messageService.showMessage({
            type: 'success',
            text: 'User deleted successfully'
          });
          this.loadUsers();
          this.loading = false;
        },
        error: (error) => {
          console.error('Error deleting user:', error);
          const errorMessage = error.error?.message ||
            error.error?.title ||
            error.message ||
            'Failed to delete user';
          this.messageService.showMessage({ type: 'error', text: errorMessage });
          this.loading = false;
        }
      });
    }
  }

  resetForm(): void {
    this.isEditMode = false;
    this.currentUserId = null;
    this.selectedRoles = [];
    this.userForm.reset({ isActive: true });

    // Reset password validators
    const passwordControl = this.userForm.get('password');
    if (passwordControl) {
      passwordControl.setValidators([
        Validators.required,
        Validators.minLength(6),
        Validators.maxLength(100)
      ]);
      passwordControl.updateValueAndValidity();
    }
  }

  // Role selection methods
  isRoleSelected(roleId: string): boolean {
    return this.selectedRoles.includes(roleId);
  }

  toggleRoleSelection(roleId: string): void {
    const index = this.selectedRoles.indexOf(roleId);
    if (index === -1) {
      this.selectedRoles.push(roleId);
    } else {
      this.selectedRoles.splice(index, 1);
    }
  }

  toggleStatus(user: any): void {
    if (!user.id || !this.authorizationService.hasPermission('users', PermissionType.AddEdit)) return;

    this.loading = true;

    // Create a simple update object with just the toggled status
    const update = {
      firstName: user.firstName,
      lastName: user.lastName,
      email: user.email,
      phoneNumber: user.phoneNumber || '',
      gender: genderToString(user.gender),
      dateOfBirth: user.dateOfBirth ? new Date(user.dateOfBirth).toISOString().split('T')[0] : null,
      bio: user.bio || '',
      isActive: !user.isActive
    };

    this.userService.updateUser(user.id, update).subscribe({
      next: () => {
        // Update the item in the local array to avoid a full reload
        user.isActive = !user.isActive;
        this.messageService.showMessage({
          type: 'success',
          text: `User ${user.isActive ? 'activated' : 'deactivated'} successfully`
        });
        this.loadUsers();
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
    this.loadUsers();
  }

  onPageSizeChange(event: Event): void {
    const selectElement = event.target as HTMLSelectElement;
    this.pageRequest.pageSize = Number(selectElement.value);
    this.pageRequest.pageNumber = 1;
    this.loadUsers();
  }

  // Sorting methods
  onSortChange(column: string): void {
    if (this.pageRequest.sortColumn === column) {
      this.pageRequest.sortDirection =
        this.pageRequest.sortDirection === 'asc' ? 'desc' : 'asc';
    } else {
      this.pageRequest.sortColumn = column;
      this.pageRequest.sortDirection = 'asc';
    }
    this.loadUsers();
  }

  // Filter methods
  onRoleFilterChange(event: Event): void {
    const selectElement = event.target as HTMLSelectElement;
    this.selectedRoleId = selectElement.value;

    // Reset to first page when filter changes
    this.pageRequest.pageNumber = 1;

    // Apply the role filter
    this.loadUsers();
  }

  onStatusFilterChange(event: Event): void {
    const selectElement = event.target as HTMLSelectElement;
    const status = selectElement.value;

    // Reset to first page when filter changes
    this.pageRequest.pageNumber = 1;

    // Apply the status filter
    this.loadUsers();
  }

  // Search method
  onSearchChange(event: Event): void {
    const target = event.target as HTMLInputElement;
    this.listService.search(target.value);
  }

  // Helper method to identify SuperAdmin by email
  private isSuperAdminEmail(email: string | undefined): boolean {
    return email === 'admin@admin.com'; // Match this with your SuperAdmin email
  }
}
