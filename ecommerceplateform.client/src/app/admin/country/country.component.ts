import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Country, CountryService } from '../../services/country.service';

@Component({
  selector: 'app-country',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './country.component.html',
  styleUrl: './country.component.scss'
})

export class CountryComponent implements OnInit {
  countries: Country[] = [];
  countryForm: FormGroup;
  isEditing = false;
  currentCountryId: string | null = null;
  isLoading = false;
  errorMessage = '';
  successMessage = '';

  constructor(
    private countryService: CountryService,
    private fb: FormBuilder
  ) {
    this.countryForm = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(100)]],
      code: ['', [Validators.required, Validators.maxLength(10)]],
      isActive: [true]
    });
  }

  ngOnInit(): void {
    this.loadCountries();
  }

  loadCountries(): void {
    this.isLoading = true;
    this.countryService.getCountries().subscribe({
      next: (data) => {
        this.countries = data;
        this.isLoading = false;
      },
      error: (error) => {
        this.errorMessage = 'Failed to load countries. Please try again.';
        this.isLoading = false;
      }
    });
  }

  onSubmit(): void {
    if (this.countryForm.invalid) {
      return;
    }

    this.isLoading = true;
    const country: Country = this.countryForm.value;

    if (this.isEditing && this.currentCountryId) {
      this.countryService.updateCountry(this.currentCountryId, country).subscribe({
        next: () => {
          this.successMessage = 'Country updated successfully!';
          this.loadCountries();
          this.resetForm();
          this.isLoading = false;
        },
        error: (error) => {
          this.errorMessage = 'Failed to update country. Please try again.';
          this.isLoading = false;
        }
      });
    } else {
      this.countryService.createCountry(country).subscribe({
        next: (newCountry) => {
          this.successMessage = 'Country created successfully!';
          this.loadCountries();
          this.resetForm();
          this.isLoading = false;
        },
        error: (error) => {
          this.errorMessage = 'Failed to create country. Please try again.';
          this.isLoading = false;
        }
      });
    }
  }

  editCountry(country: Country): void {
    this.isEditing = true;
    this.currentCountryId = country.id as string;
    this.countryForm.patchValue({
      name: country.name,
      code: country.code,
      isActive: country.isActive
    });
  }

  deleteCountry(id: string): void {
    if (confirm('Are you sure you want to delete this country?')) {
      this.isLoading = true;
      this.countryService.deleteCountry(id).subscribe({
        next: () => {
          this.successMessage = 'Country deleted successfully!';
          this.loadCountries();
          this.isLoading = false;
        },
        error: (error) => {
          this.errorMessage = 'Failed to delete country. Please try again.';
          this.isLoading = false;
        }
      });
    }
  }

  resetForm(): void {
    this.countryForm.reset({
      name: '',
      code: '',
      isActive: true
    });
    this.isEditing = false;
    this.currentCountryId = null;
  }

  clearMessages(): void {
    this.errorMessage = '';
    this.successMessage = '';
  }
} 