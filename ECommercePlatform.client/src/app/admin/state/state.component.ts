import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormsModule, ValidatorFn, AbstractControl } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { State } from '../../models/state.model';
import { Country } from '../../models/country.model';
import { StateService } from '../../services/state/state.service';
import { CountryService } from '../../services/country/country.service';
import { AuthService } from '../../services/auth/auth.service';
import { MessageService, Message } from '../../services/general/message.service';
import { Subscription } from 'rxjs';
import { NavbarComponent } from '../navbar/navbar.component';
import { CustomValidatorsService } from '../../services/custom-validators/custom-validators.service';

@Component({
  selector: 'app-state',
  templateUrl: './state.component.html',
  styleUrls: ['./state.component.scss'],
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule, FormsModule, NavbarComponent]
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

  noWhitespaceValidator(): ValidatorFn {
    return (control: AbstractControl): { [key: string]: any } | null => {
      // Check if the value exists and if it contains only whitespace
      const isWhitespace = control.value && control.value.trim().length === 0;
      // Return validation error if true, otherwise null
      return isWhitespace ? { 'whitespace': true } : null;
    };
  }

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
      name: ['', [Validators.required, Validators.maxLength(100), CustomValidatorsService.noWhitespaceValidator(), CustomValidatorsService.lettersOnly()]],
      code: ['', [Validators.required, Validators.maxLength(10), CustomValidatorsService.noWhitespaceValidator(), CustomValidatorsService.lettersOnly()]],
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
        const errorMessage = error.error?.message ||
          error.error?.title ||
          error.message ||
          'Failed to load countries';
        this.messageService.showMessage({ type: 'error', text: errorMessage });
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
        console.error('Error loading states', error);
        const errorMessage = error.error?.message ||
          error.error?.title ||
          error.message ||
          'Failed to load states';
        this.messageService.showMessage({ type: 'error', text: errorMessage });
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
        const errorMessage = error.error?.message ||
          error.error?.title ||
          error.message ||
          'Failed to load states for the selected country';
        this.messageService.showMessage({ type: 'error', text: errorMessage });
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
          const errorMessage = error.error?.message ||
            error.error?.title ||
            error.message ||
            'Failed to update State';
          this.messageService.showMessage({ type: 'error', text: errorMessage });
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
          const errorMessage = error.error?.message ||
            error.error?.title ||
            error.message ||
            'Failed to create State';
          this.messageService.showMessage({ type: 'error', text: errorMessage });
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

    // Use the message service's scrollToTop method
    this.messageService.scrollToTop();
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
          const errorMessage = error.error?.message ||
            error.error?.title ||
            error.message ||
            'Failed to delete State';
          this.messageService.showMessage({ type: 'error', text: errorMessage });
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
