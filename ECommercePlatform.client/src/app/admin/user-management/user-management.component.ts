import { Component, OnInit, OnDestroy } from '@angular/core';
import { RouterModule } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth/auth.service';
import { RoleService } from '../../services/role/role.service';
import { MessageService, Message } from '../../services/general/message.service';
import { User, UserRole } from '../../models/user.model';
import { Role, PermissionType } from '../../models/role.model';
import { Subscription } from 'rxjs';
import { PaginationComponent } from '../../shared/pagination/pagination.component';
import { PagedResponse, PagedRequest } from '../../models/pagination.model';
import { PermissionDirective } from '../../directives/permission.directive';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    PaginationComponent,
    PermissionDirective,
    RouterModule
  ]
})
export class UserManagementComponent implements OnInit, OnDestroy {
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

  userRoles = Object.values(UserRole);
  PermissionType = PermissionType;
  Math = Math;

  // Pagination properties
  pagedResponse: PagedResponse<User> | null = null;
  pageRequest: PagedRequest = {
    pageNumber: 1,
    pageSize: 10,
    searchText: '',
    sortColumn: 'email',
    sortDirection: 'asc'
  };

  private currentUser: any = null;
  private subscriptions: Subscription[] = [];

  constructor(
    private authService: AuthService,
    private roleService: RoleService,
    private messageService: MessageService,
    private fb: FormBuilder
  ) { }

  ngOnInit(): void {
    this.initForm();
    this.loadUsers();
    this.loadRoles();

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

  private initForm(): void {
    this.userForm = this.fb.group({
      firstName: ['', [Validators.required, Validators.maxLength(50)]],
      lastName: ['', [Validators.required, Validators.maxLength(50)]],
      email: ['', [Validators.required, Validators.email, Validators.maxLength(100)]],
      password: ['', [Validators.minLength(6), Validators.maxLength(100)]],
      role: [UserRole.Customer, Validators.required],
      isActive: [true]
    });

    // Only require password when creating a new user
    this.userForm.get('password')?.setValidators(
      this.isEditMode ? [Validators.minLength(6), Validators.maxLength(100)] :
        [Validators.required, Validators.minLength(6), Validators.maxLength(100)]
    );
    this.userForm.get('password')?.updateValueAndValidity();
  }

  loadUsers(): void {
    this.loading = true;
    const sub = this.authService.getUsers(this.pageRequest).subscribe({
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
    const sub = this.roleService.getRoles().subscribe({
      next: (roles) => {
        this.availableRoles = roles;
      },
      error: (error) => {
        console.error('Error loading roles:', error);
      }
    });

    this.subscriptions.push(sub);
  }

  loadUserRoles(userId: string): void {
    const sub = this.roleService.getUserRoles(userId).subscribe({
      next: (roles) => {
        this.selectedRoles = roles.map(role => role.id as string);
      },
      error: (error) => {
        console.error('Error loading user roles:', error);
      }
    });

    this.subscriptions.push(sub);
  }

  onSubmit(): void {
    if (this.userForm.invalid) {
      return;
    }

    const userData: User = {
      ...this.userForm.value,
      id: this.isEditMode && this.currentUserId ? this.currentUserId : undefined
    };

    this.loading = true;

    if (this.isEditMode && this.currentUserId) {
      const sub = this.authService.updateUser(this.currentUserId, userData).subscribe({
        next: () => {
          this.messageService.showMessage({ type: 'success', text: 'User updated successfully' });

          // If roles were selected, assign them
          if (this.selectedRoles.length > 0) {
            this.assignRolesToUser(this.currentUserId as string);
          } else {
            this.loadUsers();
            this.resetForm();
            this.loading = false;
          }
        },
        error: (error) => {
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
      const sub = this.authService.createUser(userData).subscribe({
        next: (newUser) => {
          this.messageService.showMessage({ type: 'success', text: 'User created successfully' });

          // If roles were selected, assign them
          if (this.selectedRoles.length > 0) {
            this.assignRolesToUser(newUser.id as string);
          } else {
            this.loadUsers();
            this.resetForm();
            this.loading = false;
          }
        },
        error: (error) => {
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

  assignRolesToUser(userId: string): void {
    const assignment = {
      userId: userId,
      roleIds: this.selectedRoles
    };

    const sub = this.roleService.assignRolesToUser(assignment).subscribe({
      next: () => {
        this.loadUsers();
        this.resetForm();
        this.loading = false;
      },
      error: (error) => {
        console.error('Error assigning roles:', error);
        this.messageService.showMessage({
          type: 'error',
          text: error.error?.message || 'Failed to assign roles'
        });
        this.loading = false;
      }
    });

    this.subscriptions.push(sub);
  }

  editUser(user: User): void {
    this.isEditMode = true;
    this.currentUserId = user.id || null;

    this.userForm.patchValue({
      firstName: user.firstName,
      lastName: user.lastName,
      email: user.email,
      role: user.role,
      isActive: user.isActive
    });

    // Clear password field in edit mode
    this.userForm.get('password')?.setValue('');
    this.userForm.get('password')?.clearValidators();
    this.userForm.get('password')?.setValidators([Validators.minLength(6), Validators.maxLength(100)]);
    this.userForm.get('password')?.updateValueAndValidity();

    // Load user roles
    if (user.id) {
      this.loadUserRoles(user.id);
    }

    this.messageService.scrollToTop();
  }

  deleteUser(id: string | undefined): void {
    if (!id) {
      this.messageService.showMessage({
        type: 'error',
        text: 'Cannot delete user: User ID is missing'
      });
      return;
    }

    if (confirm('Are you sure you want to delete this user?')) {
      this.loading = true;
      const sub = this.authService.deleteUser(id).subscribe({
        next: () => {
          this.messageService.showMessage({ type: 'success', text: 'User deleted successfully' });
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

      this.subscriptions.push(sub);
    }
  }

  resetForm(): void {
    this.userForm.reset({
      role: UserRole.Customer,
      isActive: true
    });
    this.selectedRoles = [];
    this.isEditMode = false;
    this.currentUserId = null;

    // Reset password validation for new user
    this.userForm.get('password')?.setValidators([
      Validators.required,
      Validators.minLength(6),
      Validators.maxLength(100)
    ]);
    this.userForm.get('password')?.updateValueAndValidity();
  }

  toggleRoleSelection(roleId: string | undefined): void {
    if (!roleId) return;

    const index = this.selectedRoles.indexOf(roleId);
    if (index === -1) {
      this.selectedRoles.push(roleId);
    } else {
      this.selectedRoles.splice(index, 1);
    }
  }

  isRoleSelected(roleId: string | undefined): boolean {
    if (!roleId) return false;
    return this.selectedRoles.includes(roleId);
  }

  onSearchChange(event: Event): void {
    const searchTerm = (event.target as HTMLInputElement).value;
    this.pageRequest.searchText = searchTerm;
    this.pageRequest.pageNumber = 1;
    this.loadUsers();
  }

  // Pagination methods
  onPageChange(page: number | Event): void {
    // Handle both number and Event types
    const pageNumber = typeof page === 'number' ? page : 1;
    this.pageRequest.pageNumber = pageNumber;
    this.loadUsers();
  }

  onPageSizeChange(event: Event): void {
    const selectElement = event.target as HTMLSelectElement;
    this.pageRequest.pageSize = +selectElement.value;
    this.pageRequest.pageNumber = 1;
    this.loadUsers();
  }

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
}
