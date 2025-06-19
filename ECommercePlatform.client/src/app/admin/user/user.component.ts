import { Component, OnInit, OnDestroy } from '@angular/core';
import { RouterModule } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { UserService } from '../../services/user/user.service';
import { RoleService } from '../../services/role/role.service';
import { MessageService, Message } from '../../services/general/message.service';
import { User } from '../../models/user.model';
import { Role, PermissionType } from '../../models/role.model';
import { Subscription } from 'rxjs';
import { PaginationComponent } from '../../shared/pagination/pagination.component';
import { PagedResponse, PagedRequest } from '../../models/pagination.model';
import { PermissionDirective } from '../../directives/permission.directive';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-user',
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    PaginationComponent,
    PermissionDirective,
    RouterModule
  ]
})
export class UserComponent implements OnInit, OnDestroy {
  users: User[] = [];
  roles: Role[] = [];
  userForm!: FormGroup;
  isEditMode: boolean = false;
  currentUserId: string | null = null;
  loading: boolean = false;
  message: Message | null = null;

  // User roles assignment
  selectedRoles: string[] = [];
  availableRoles: Role[] = [];

  PermissionType = PermissionType;
  Math = Math;
  rolesLoading: boolean = false;

  // Pagination properties
  pagedResponse: PagedResponse<User> | null = null;
  pageRequest: PagedRequest = {
    pageNumber: 1,
    pageSize: 10,
    searchText: '',
    sortColumn: 'email',
    sortDirection: 'asc'
  };

  private subscriptions: Subscription[] = [];

  constructor(
    private userService: UserService,
    private roleService: RoleService,
    private messageService: MessageService,
    private fb: FormBuilder
  ) { }

  ngOnInit(): void {
    this.initForm();
    this.loadUsers();
    this.loadRoles();

    const messageSub = this.messageService.currentMessage.subscribe(message => {
      this.message = message;
    });

    this.subscriptions.push(messageSub);
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  private initForm(): void {
    this.userForm = this.fb.group({
      firstName: ['', [Validators.required, Validators.maxLength(50)]],
      lastName: ['', [Validators.required, Validators.maxLength(50)]],
      email: ['', [Validators.required, Validators.email, Validators.maxLength(100)]],
      password: ['', [Validators.minLength(6), Validators.maxLength(100)]],
      phoneNumber: ['', [Validators.maxLength(20)]],
      gender: [''],
      dateOfBirth: [''],
      bio: ['', [Validators.maxLength(500)]],
      isActive: [true]
    });

    // Set password validators based on mode
    this.updatePasswordValidators();
    //// Only require password when creating a new user
    //this.userForm.get('password')?.setValidators(
    //  this.isEditMode ? [Validators.minLength(6), Validators.maxLength(100)] :
    //    [Validators.required, Validators.minLength(6), Validators.maxLength(100)]
    //);
    //this.userForm.get('password')?.updateValueAndValidity();
  }

  loadUsers(): void {
    this.loading = true;
    const sub = this.userService.getPagedUsers(this.pageRequest).subscribe({
      next: (response) => {
        this.pagedResponse = response;
        this.users = response.items;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading users:', error);
        this.messageService.showMessage({
          type: 'error',
          text: error.error?.message || 'Failed to load users'
        });
        this.loading = false;
      }
    });

    this.subscriptions.push(sub);
  }

  loadRoles(): void {
    this.rolesLoading = true;
    const sub = this.roleService.getRoles().subscribe({
      next: (roles) => {
        this.availableRoles = roles.filter(r => r.isActive);
        this.rolesLoading = false;
      },
      error: (error) => {
        console.error('Error loading roles:', error);
        this.messageService.showMessage({
          type: 'error',
          text: error.error?.message || 'Failed to load roles'
        });
        this.rolesLoading = false;
      }
    });
    this.subscriptions.push(sub);
  }

  onSubmit(): void {
    if (this.userForm.invalid) {
      // Mark all fields as touched to show validation errors
      Object.keys(this.userForm.controls).forEach(key => {
        this.userForm.get(key)?.markAsTouched();
      });
      return;
    }

    const userData = {
      ...this.userForm.value,
      roleIds: this.selectedRoles
    };

    // Remove empty password in edit mode
    if (this.isEditMode && !userData.password) {
      delete userData.password;
    }

    this.loading = true;

    if (this.isEditMode && this.currentUserId) {
      const sub = this.userService.updateUser(this.currentUserId, userData).subscribe({
        next: () => {
          this.messageService.showMessage({
            type: 'success',
            text: 'User updated successfully'
          });
          this.resetForm();
          this.loadUsers();
          this.loading = false;
        },
        error: (error: any) => {
          console.error('Error updating user:', error);
          this.messageService.showMessage({
            type: 'error',
            text: error.error?.message || 'Failed to update user'
          });
          this.loading = false;
        }
      });
      this.subscriptions.push(sub);
    } else {
      const sub = this.userService.createUser(userData).subscribe({
        next: (user: User) => {
          this.messageService.showMessage({
            type: 'success',
            text: 'User created successfully'
          });
          this.resetForm();
          this.loadUsers();
          this.loading = false;
        },
        error: (error: any) => {
          console.error('Error creating user:', error);
          this.messageService.showMessage({
            type: 'error',
            text: error.error?.message || 'Failed to create user'
          });
          this.loading = false;
        }
      });
      this.subscriptions.push(sub);
    }
  }

  editUser(user: User): void {
    this.isEditMode = true;
    this.currentUserId = user.id || null;

    // First, get the full user details with roles
    if (user.id) {
      this.loading = true;
      const sub = this.userService.getUserWithRoles(user.id).subscribe({
        next: (fullUser) => {
          this.userForm.patchValue({
            firstName: fullUser.firstName,
            lastName: fullUser.lastName,
            email: fullUser.email,
            phoneNumber: fullUser.phoneNumber,
            gender: fullUser.gender || '',
            dateOfBirth: fullUser.dateOfBirth ? this.formatDateForInput(fullUser.dateOfBirth) : '',
            bio: fullUser.bio || '',
            isActive: fullUser.isActive
          });

          // Clear password field
          this.userForm.get('password')?.setValue('');
          this.updatePasswordValidators();

          // Set selected roles
          this.selectedRoles = fullUser.roles?.map(role => role.id).filter((id): id is string => id !== undefined) || [];

          this.loading = false;
          this.messageService.scrollToTop();
        },
        error: (error) => {
          console.error('Error loading user details:', error);
          this.messageService.showMessage({
            type: 'error',
            text: 'Failed to load user details'
          });
          this.loading = false;
        }
      });
      this.subscriptions.push(sub);
    }
  }

  // Helper method to format date for input
  private formatDateForInput(date: any): string {
    if (!date) return '';
    const d = new Date(date);
    return d.toISOString().split('T')[0];
  }

  deleteUser(id: string | undefined): void {
    if (!id) {
      this.messageService.showMessage({
        type: 'error',
        text: 'Cannot delete user: Invalid ID'
      });
      return;
    }

    if (confirm('Are you sure you want to delete this user?')) {
      this.loading = true;

      this.userService.deleteUser(id).subscribe({
        next: () => {
          this.messageService.showMessage({
            type: 'success',
            text: 'User deleted successfully'
          });
          this.resetForm();
          this.loadUsers();
          this.loading = false;
        },
        error: (error) => {
          console.error('Error deleting user:', error);
          this.messageService.showMessage({
            type: 'error',
            text: error.error?.message || 'Failed to delete user'
          });
          this.loading = false;
        }
      });
    }
  }

  resetForm(): void {
    this.userForm.reset({
      isActive: true,
      gender: '',
      dateOfBirth: ''
    });
    this.selectedRoles = [];
    this.isEditMode = false;
    this.currentUserId = null;
    this.updatePasswordValidators();
  }

  private updatePasswordValidators(): void {
    const passwordControl = this.userForm.get('password');
    if (this.isEditMode) {
      passwordControl?.setValidators([Validators.minLength(6), Validators.maxLength(100)]);
    } else {
      passwordControl?.setValidators([Validators.required, Validators.minLength(6), Validators.maxLength(100)]);
    }
    passwordControl?.updateValueAndValidity();
  }

  toggleRoleSelection(roleId: string): void {
    const index = this.selectedRoles.indexOf(roleId);
    if (index === -1) {
      this.selectedRoles.push(roleId);
    } else {
      this.selectedRoles.splice(index, 1);
    }
  }

  isRoleSelected(roleId: string): boolean {
    return this.selectedRoles.includes(roleId);
  }

  onPageChange(page: number): void {
    this.pageRequest.pageNumber = page;
    this.loadUsers();
  }

  onSortChange(column: string): void {
    if (this.pageRequest.sortColumn === column) {
      // Toggle sort direction if clicking on the same column
      this.pageRequest.sortDirection = this.pageRequest.sortDirection === 'asc' ? 'desc' : 'asc';
    } else {
      // Default to ascending for a new column
      this.pageRequest.sortColumn = column;
      this.pageRequest.sortDirection = 'asc';
    }
    this.loadUsers();
  }

  onSearchChange(event: Event): void {
    const target = event.target as HTMLInputElement;
    this.pageRequest.searchText = target.value;

    // Reset to first page when searching
    this.pageRequest.pageNumber = 1;

    // Debounce search
    clearTimeout(this.searchTimeout);
    this.searchTimeout = setTimeout(() => {
      this.loadUsers();
    }, 500);
  }

  onPageSizeChange(event: Event): void {
    const selectElement = event.target as HTMLSelectElement;
    this.pageRequest.pageSize = Number(selectElement.value);
    this.pageRequest.pageNumber = 1; // Reset to first page
    this.loadUsers();
  }

  private searchTimeout: any;
}
