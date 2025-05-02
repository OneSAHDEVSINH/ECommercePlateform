import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { City, CityService } from '../../services/city.service';
import { State, StateService } from '../../services/state.service';
import { Country, CountryService } from '../../services/country.service';

@Component({
  selector: 'app-city',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './city.component.html',
  styleUrl: './city.component.scss'
})

export class CityComponent implements OnInit {
  cities: City[] = [];
  states: State[] = [];
  countries: Country[] = [];
  cityForm: FormGroup;
  isEditing = false;
  currentCityId: string | null = null;
  isLoading = false;
  errorMessage = '';
  successMessage = '';
  selectedCountryId: string | null = null;

  constructor(
    private cityService: CityService,
    private stateService: StateService,
    private countryService: CountryService,
    private fb: FormBuilder
  ) {
    this.cityForm = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(100)]],
      stateId: ['', [Validators.required]],
      isActive: [true]
    });
  }

  ngOnInit(): void {
    this.loadCountries();
    this.loadCities();
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

  loadStates(countryId?: string): void {
    if (countryId) {
      this.stateService.getStatesByCountry(countryId).subscribe({
        next: (data) => {
          this.states = data;
        },
        error: () => {
          this.errorMessage = 'Failed to load states. Please try again.';
        }
      });
    } else {
      this.stateService.getStates().subscribe({
        next: (data) => {
          this.states = data;
        },
        error: () => {
          this.errorMessage = 'Failed to load states. Please try again.';
        }
      });
    }
  }

  loadCities(): void {
    this.isLoading = true;
    this.cityService.getCities().subscribe({
      next: (data) => {
        this.cities = data;
        this.isLoading = false;
      },
      error: () => {
        this.errorMessage = 'Failed to load cities. Please try again.';
        this.isLoading = false;
      }
    });
  }

  onCountryChange(event: Event): void {
    const select = event.target as HTMLSelectElement;
    const countryId = select.value;
    this.selectedCountryId = countryId || null;
    
    if (countryId) {
      this.loadStates(countryId);
      this.cityForm.patchValue({ stateId: '' });
    } else {
      this.states = [];
      this.cityForm.patchValue({ stateId: '' });
    }
  }

  onSubmit(): void {
    if (this.cityForm.invalid) {
      return;
    }

    this.isLoading = true;
    const city: City = this.cityForm.value;

    if (this.isEditing && this.currentCityId) {
      this.cityService.updateCity(this.currentCityId, city).subscribe({
        next: () => {
          this.successMessage = 'City updated successfully!';
          this.loadCities();
          this.resetForm();
          this.isLoading = false;
        },
        error: () => {
          this.errorMessage = 'Failed to update city. Please try again.';
          this.isLoading = false;
        }
      });
    } else {
      this.cityService.createCity(city).subscribe({
        next: () => {
          this.successMessage = 'City created successfully!';
          this.loadCities();
          this.resetForm();
          this.isLoading = false;
        },
        error: () => {
          this.errorMessage = 'Failed to create city. Please try again.';
          this.isLoading = false;
        }
      });
    }
  }

  editCity(city: City): void {
    this.isEditing = true;
    this.currentCityId = city.id as string;
    
    // First load the state's country
    if (city.state && city.state.countryId) {
      this.selectedCountryId = city.state.countryId;
      this.loadStates(city.state.countryId);
    }
    
    this.cityForm.patchValue({
      name: city.name,
      stateId: city.stateId,
      isActive: city.isActive
    });
  }

  deleteCity(id: string): void {
    if (confirm('Are you sure you want to delete this city?')) {
      this.isLoading = true;
      this.cityService.deleteCity(id).subscribe({
        next: () => {
          this.successMessage = 'City deleted successfully!';
          this.loadCities();
          this.isLoading = false;
        },
        error: () => {
          this.errorMessage = 'Failed to delete city. Please try again.';
          this.isLoading = false;
        }
      });
    }
  }

  getStateName(stateId: string): string {
    const state = this.states.find(s => s.id === stateId);
    return state ? state.name : 'Unknown';
  }

  getCountryForState(stateId: string): string {
    const state = this.states.find(s => s.id === stateId);
    if (state && state.country) {
      return state.country.name;
    }
    return 'Unknown';
  }

  resetForm(): void {
    this.cityForm.reset({
      name: '',
      stateId: '',
      isActive: true
    });
    this.isEditing = false;
    this.currentCityId = null;
    this.selectedCountryId = null;
  }

  clearMessages(): void {
    this.errorMessage = '';
    this.successMessage = '';
  }
} 