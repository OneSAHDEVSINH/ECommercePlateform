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
    this.roleService.getRoles().subscribe({
      next: (roles) => {
        this.availableRoles = roles;
      },
      error: (error) => {
        console.error('Error loading roles:', error);
        this.messageService.showMessage({
          type: 'error',
          text: error.error?.message || 'Failed to load roles'
        });
      }
    });
  }

  onSubmit(): void {
    if (this.userForm.invalid) return;

    const userData = {
      ...this.userForm.value,
      roleIds: this.selectedRoles
    };

    this.loading = true;

    if (this.isEditMode && this.currentUserId) {
      this.userService.updateUser(this.currentUserId, userData).subscribe({
        next: () => {
          this.messageService.showMessage({
            type: 'success',
            text: 'User updated successfully'
          });
          this.resetForm();
          this.loadUsers();
          this.loading = false;
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
    } else {
      this.userService.createUser(userData).subscribe({
        next: () => {
          this.messageService.showMessage({
            type: 'success',
            text: 'User created successfully'
          });
          this.resetForm();
          this.loadUsers();
          this.loading = false;
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
    }
  }

  editUser(user: User): void {
    this.isEditMode = true;
    this.currentUserId = user.id || null; 
    this.userForm.patchValue({
      firstName: user.firstName,
      lastName: user.lastName,
      email: user.email,
      phoneNumber: user.phoneNumber,
      isActive: user.isActive
    });

    // Clear password as it's not returned from the API
    this.userForm.get('password')?.setValue('');
    this.userForm.get('password')?.clearValidators();
    this.userForm.get('password')?.setValidators([Validators.minLength(6), Validators.maxLength(100)]);
    this.userForm.get('password')?.updateValueAndValidity();

    this.selectedRoles = user.roles?.map(role => role.id).filter((id): id is string => id !== undefined) || [];
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
    this.userForm.reset({ isActive: true });
    this.selectedRoles = [];
    this.isEditMode = false;
    this.currentUserId = null;

    // Reset password validation
    this.userForm.get('password')?.setValidators([
      Validators.required,
      Validators.minLength(6),
      Validators.maxLength(100)
    ]);
    this.userForm.get('password')?.updateValueAndValidity();
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

  private searchTimeout: any;
}
