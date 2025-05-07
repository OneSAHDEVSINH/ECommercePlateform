import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { State } from '../../models/state.model';
import { Country } from '../../models/country.model';
import { StateService } from '../../services/state.service';
import { CountryService } from '../../services/country.service';
import { AuthService } from '../../services/auth.service';
import { MessageService, Message } from '../../services/message.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-state',
  templateUrl: './state.component.html',
  styleUrls: ['./state.component.scss'],
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule, FormsModule]
})
export class StateComponent implements OnInit {
  states: State[] = [];
  countries: Country[] = [];
  stateForm!: FormGroup;
  isEditMode: boolean = false;
  currentStateId: string | null = null;
  loading: boolean = false;
  message: Message | null = null;
  private currentUser: any = null;
  private messageSubscription!: Subscription;

  constructor(
    private stateService: StateService,
    private countryService: CountryService,
    private authService: AuthService,
    private messageService: MessageService,
    private fb: FormBuilder
  ) { }

  ngOnInit(): void {
    this.initForm();
    this.loadCountries();
    this.loadStates();
    this.authService.currentUser$.subscribe(user => {
      this.currentUser = user;
    });

    // Subscribe to message changes
    this.messageSubscription = this.messageService.currentMessage.subscribe(message => {
      this.message = message;
    });
  }

  ngOnDestroy(): void {
    // Clean up subscriptions
    if (this.messageSubscription) {
      this.messageSubscription.unsubscribe();
    }
  }

  private initForm(): void {
    this.stateForm = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(100)]],
      code: ['', [Validators.required, Validators.maxLength(10)]],
      countryId: ['', [Validators.required]]
    });
  }

  loadCountries(): void {
    this.countryService.getCountries().subscribe({
      next: (countries) => {
        this.countries = countries;
      },
      error: (error) => {
        console.error('Error loading countries:', error);
        this.messageService.showMessage({ type: 'error', text: 'Failed to load countries' });
      }
    });
  }

  loadStates(): void {
    this.loading = true;
    this.stateService.getStates().subscribe({
      next: (states) => {
        this.states = states;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading states:', error);
        this.messageService.showMessage({ type: 'error', text: 'Failed to load states' });
        this.loading = false;
      }
    });
  }

  filterStatesByCountry(countryId: string): void {
    if (!countryId || countryId === 'all') {
      this.loadStates();
      return;
    }

    this.loading = true;
    this.stateService.getStatesByCountry(countryId).subscribe({
      next: (states) => {
        this.states = states;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading states by country:', error);
        this.messageService.showMessage({ type: 'error', text: 'Failed to load states for the selected country' });
        this.loading = false;
      }
    });
  }

  onCountryFilterChange(event: Event): void {
    const select = event.target as HTMLSelectElement;
    this.filterStatesByCountry(select.value);
  }

  onSubmit(): void {
    if (this.stateForm.invalid) {
      return;
    }

    const stateData: State = {
      ...this.stateForm.value,
      id: this.isEditMode && this.currentStateId ? this.currentStateId : undefined,
      createdBy: this.isEditMode ? undefined : this.getUserIdentifier(),
      modifiedBy: this.getUserIdentifier(),
      isActive: true,
      isDeleted: false
    };

    this.loading = true;
    
    if (this.isEditMode && this.currentStateId) {
      this.stateService.updateState(this.currentStateId, stateData).subscribe({
        next: () => {
          this.messageService.showMessage({ type: 'success', text: 'State updated successfully' });
          this.loadStates();
          this.resetForm();
          this.loading = false;
        },
        error: (error) => {
          console.error('Error updating state:', error);
          this.messageService.showMessage({ type: 'error', text: 'Failed to update state' });
          this.loading = false;
        }
      });
    } else {
      this.stateService.createState(stateData).subscribe({
        next: () => {
          this.messageService.showMessage({ type: 'success', text: 'State created successfully' });
          this.loadStates();
          this.resetForm();
          this.loading = false;
        },
        error: (error) => {
          console.error('Error creating state:', error);
          this.messageService.showMessage({ type: 'error', text: 'Failed to create state' });
          this.loading = false;
        }
      });
    }
  }

  editState(state: State): void {
    this.isEditMode = true;
    this.currentStateId = state.id || null;
    this.stateForm.patchValue({
      name: state.name,
      code: state.code,
      countryId: state.countryId,
      modifiedOn: new Date(),
      modifiedBy: this.getUserIdentifier(),
      isActive: true,
      isDeleted: false
    });
  }

  deleteState(id: string): void {
    if (confirm('Are you sure you want to delete this state?')) {
      this.loading = true;
      this.stateService.deleteState(id).subscribe({
        next: () => {
          this.messageService.showMessage({ type: 'success', text: 'State deleted successfully' });
          this.loadStates();
          this.loading = false;
        },
        error: (error) => {
          console.error('Error deleting state:', error);
          this.messageService.showMessage({ type: 'error', text: 'Failed to delete state' });
          this.loading = false;
        }
      });
    }
  }

  resetForm(): void {
    this.stateForm.reset();
    this.isEditMode = false;
    this.currentStateId = null;
  }

  getCountryName(countryId: string): string {
    const country = this.countries.find(c => c.id === countryId);
    return country ? country.name : 'Unknown';
  }

  // Helper method to get user identifier for creation/modification tracking
  private getUserIdentifier(): string {
    return this.currentUser ? this.currentUser.id || this.currentUser.email : 'system';
  }
}
