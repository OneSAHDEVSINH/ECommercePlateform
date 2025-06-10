import { Component, OnInit, OnDestroy } from '@angular/core';
import { RouterModule } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ModuleService } from '../../services/module/module.service';
import { MessageService, Message } from '../../services/general/message.service';
import { Module } from '../../models/role.model';
import { Subscription } from 'rxjs';
import { PaginationComponent } from '../../shared/pagination/pagination.component';
import { PermissionDirective } from '../../directives/permission.directive';

@Component({
  selector: 'app-module-management',
  templateUrl: './module-management.component.html',
  styleUrls: ['./module-management.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    PaginationComponent,
    PermissionDirective,
    RouterModule
  ]
})
export class ModuleManagementComponent implements OnInit, OnDestroy {
  modules: Module[] = [];
  moduleForm!: FormGroup;
  isEditMode: boolean = false;
  currentModuleId: string | null = null;
  loading: boolean = false;
  message: Message | null = null;

  private subscriptions: Subscription[] = [];

  constructor(
    private moduleService: ModuleService,
    private messageService: MessageService,
    private fb: FormBuilder
  ) { }

  ngOnInit(): void {
    this.initForm();
    this.loadModules();

    const messageSub = this.messageService.currentMessage.subscribe(message => {
      this.message = message;
    });

    this.subscriptions.push(messageSub);
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  private initForm(): void {
    this.moduleForm = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(100)]],
      description: ['', [Validators.maxLength(500)]],
      route: ['', [Validators.required, Validators.maxLength(50)]],
      icon: ['', [Validators.maxLength(50)]],
      displayOrder: [0, [Validators.required, Validators.min(0)]],
      isActive: [true]
    });
  }

  loadModules(): void {
    this.loading = true;
    const sub = this.moduleService.getModules().subscribe({
      next: (modules) => {
        this.modules = modules;
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

  onSubmit(): void {
    if (this.moduleForm.invalid) {
      return;
    }

    this.loading = true;
    const moduleData = this.moduleForm.value;

    if (this.isEditMode && this.currentModuleId) {
      const sub = this.moduleService.updateModule(this.currentModuleId, moduleData).subscribe({
        next: () => {
          this.messageService.showMessage({ type: 'success', text: 'Module updated successfully' });
          this.loadModules();
          this.resetForm();
          this.loading = false;
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
          this.messageService.showMessage({ type: 'success', text: 'Module created successfully' });
          this.loadModules();
          this.resetForm();
          this.loading = false;
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

    if (confirm('Are you sure you want to delete this module?')) {
      this.loading = true;
      const sub = this.moduleService.deleteModule(id).subscribe({
        next: () => {
          this.messageService.showMessage({ type: 'success', text: 'Module deleted successfully' });
          this.loadModules();
          this.loading = false;
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
}
