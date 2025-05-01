import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { State, StateService } from '../../services/state.service';
import { Country, CountryService } from '../../services/country.service';

@Component({
  selector: 'app-state',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './state.component.html',
  styleUrl: './state.component.scss'
})
export class StateComponent implements OnInit {
  states: State[] = [];
  countries: Country[] = [];
  stateForm: FormGroup;
  isEditing = false;
  currentStateId: string | null = null;
  isLoading = false;
  errorMessage = '';
  successMessage = '';

  constructor(
    private stateService: StateService,
    private countryService: CountryService,
    private fb: FormBuilder
  ) {
    this.stateForm = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(100)]],
      code: ['', [Validators.required, Validators.maxLength(10)]],
      countryId: ['', [Validators.required]],
      isActive: [true]
    });
  }

  ngOnInit(): void {
    this.loadCountries();
    this.loadStates();
  }

  loadCountries(): void {
    this.countryService.getCountries().subscribe({
      next: (data) => {
        this.countries = data;
      },
      error: () => {
        this.errorMessage = 'Failed to load countries. Please try again.';
      }
    });
  }

  loadStates(): void {
    this.isLoading = true;
    this.stateService.getStates().subscribe({
      next: (data) => {
        this.states = data;
        this.isLoading = false;
      },
      error: () => {
        this.errorMessage = 'Failed to load states. Please try again.';
        this.isLoading = false;
      }
    });
  }

  onSubmit(): void {
    if (this.stateForm.invalid) {
      return;
    }

    this.isLoading = true;
    const state: State = this.stateForm.value;

    if (this.isEditing && this.currentStateId) {
      this.stateService.updateState(this.currentStateId, state).subscribe({
        next: () => {
          this.successMessage = 'State updated successfully!';
          this.loadStates();
          this.resetForm();
          this.isLoading = false;
        },
        error: () => {
          this.errorMessage = 'Failed to update state. Please try again.';
          this.isLoading = false;
        }
      });
    } else {
      this.stateService.createState(state).subscribe({
        next: () => {
          this.successMessage = 'State created successfully!';
          this.loadStates();
          this.resetForm();
          this.isLoading = false;
        },
        error: () => {
          this.errorMessage = 'Failed to create state. Please try again.';
          this.isLoading = false;
        }
      });
    }
  }

  editState(state: State): void {
    this.isEditing = true;
    this.currentStateId = state.id as string;
    this.stateForm.patchValue({
      name: state.name,
      code: state.code,
      countryId: state.countryId,
      isActive: state.isActive
    });
  }

  deleteState(id: string): void {
    if (confirm('Are you sure you want to delete this state?')) {
      this.isLoading = true;
      this.stateService.deleteState(id).subscribe({
        next: () => {
          this.successMessage = 'State deleted successfully!';
          this.loadStates();
          this.isLoading = false;
        },
        error: () => {
          this.errorMessage = 'Failed to delete state. Please try again.';
          this.isLoading = false;
        }
      });
    }
  }

  getCountryName(countryId: string): string {
    const country = this.countries.find(c => c.id === countryId);
    return country ? country.name : 'Unknown';
  }

  resetForm(): void {
    this.stateForm.reset({
      name: '',
      code: '',
      countryId: '',
      isActive: true
    });
    this.isEditing = false;
    this.currentStateId = null;
  }

  clearMessages(): void {
    this.errorMessage = '';
    this.successMessage = '';
  }
} 