import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, ValidatorFn, AbstractControl } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { Country } from '../../models/country.model';
import { CountryService } from '../../services/country.service';
import { AuthService } from '../../services/auth.service';
import { MessageService, Message } from '../../services/message.service';
import { Subscription } from 'rxjs';
import { NavbarComponent } from "../navbar/navbar.component";
import { CustomValidatorsService } from '../../services/custom-validators.service';

@Component({
  selector: 'app-country',
  templateUrl: './country.component.html',
  styleUrls: ['./country.component.scss'],
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule, NavbarComponent, NavbarComponent]
})

export class CountryComponent implements OnInit, OnDestroy {
  countries: Country[] = [];
  countryForm!: FormGroup;
  isEditMode: boolean = false;
  currentCountryId: string | null = null;
  loading: boolean = false;
  message: Message | null = null;
  private currentUser: any = null;
  private messageSubscription!: Subscription;


  constructor(
    private countryService: CountryService,
    private authService: AuthService,
    private messageService: MessageService,
    private fb: FormBuilder,
    private nws: NoWhiteSpaceService
  ) { }

  ngOnInit(): void {
    this.initForm();
    this.loadCountries();
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
    this.countryForm = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(100), CustomValidatorsService.noWhitespaceValidator(), CustomValidatorsService.lettersOnly()]],
      code: ['', [Validators.required, Validators.maxLength(10), CustomValidatorsService.noWhitespaceValidator(), , CustomValidatorsService.lettersOnly()]]
    });
  }

  loadCountries(): void {
    this.loading = true;
    this.countryService.getCountries().subscribe({
      next: (countries) => {
        this.countries = countries;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading countries:', error);
        this.messageService.showMessage({ type: 'error', text: 'Failed to load countries' });
        this.loading = false;
      }
    });
  }

  onSubmit(): void {

    if (this.countryForm.invalid) {
      return;
    }

    const countryData: Country = {
      ...this.countryForm.value,
      //createdBy: this.isEditMode ? undefined : this.getUserIdentifier(),
      //modifiedBy: this.getUserIdentifier(),
      //isActive: true,
      //isDeleted: false
      id: this.isEditMode && this.currentCountryId ? this.currentCountryId : undefined,
      //name: this.countryForm.value.name,
      //code: this.countryForm.value.code,
      createdOn: new Date(),
      createdBy: this.isEditMode ? undefined : this.getUserIdentifier(),
      //createdBy: "System",
      modifiedOn: new Date(),
      modifiedBy: this.getUserIdentifier(),
      //modifiedBy: "System",
      isActive: true,
      isDeleted: false
    };

    this.loading = true;

    console.log('Country data being sent:', JSON.stringify(countryData));

    if (this.isEditMode && this.currentCountryId) {
      this.countryService.updateCountry(this.currentCountryId, countryData).subscribe({
        next: () => {
          this.messageService.showMessage({ type: 'success', text: 'Country updated successfully' });
          this.loadCountries();
          this.resetForm();
          this.loading = false;
        },
        error: (error) => {
          console.error('Error updating country:', error);
          // And in the updateCountry method's error handler
          if (error.error) {
            console.error('Server validation errors:', error.error);
            console.error('Request payload:', countryData);
          }
          // Extract the most useful error message
          const errorMessage = error.error?.message ||
            error.error?.title ||
            error.message ||
            'Failed to update country';
          this.messageService.showMessage({ type: 'error', text: errorMessage });
          this.loading = false;
        }
      });
    } else {
      this.countryService.createCountry(countryData).subscribe({
        next: () => {
          this.messageService.showMessage({ type: 'success', text: 'Country created successfully' });
          this.loadCountries();
          this.resetForm();
          this.loading = false;
        },
        error: (error) => {
          console.error('Error creating country:', error);
          // Log more details about the error response
          if (error.error) {
            console.error('Server validation errors:', error.error);
          }
          // Extract the most useful error message
          const errorMessage = error.error?.message ||
            error.error?.title ||
            error.message ||
            'Failed to create country';
          this.messageService.showMessage({ type: 'error', text: errorMessage });
          this.loading = false;
        }
      });
    }
  }

  editCountry(country: Country): void {
    this.isEditMode = true;
    this.currentCountryId = country.id || null;
    this.countryForm.patchValue({
      name: country.name,
      code: country.code,
      //createdOn: new Date(),
      //createdBy: this.isEditMode ? undefined : this.getUserIdentifier(),
      modifiedOn: new Date(),
      modifiedBy: this.getUserIdentifier(),
      isActive: true,
      isDeleted: false
    });
  }

  deleteCountry(id: string): void {
    if (confirm('Are you sure you want to delete this country?')) {
      this.loading = true;
      this.countryService.deleteCountry(id).subscribe({
        next: () => {
          this.messageService.showMessage({ type: 'success', text: 'Country deleted successfully' });
          this.loadCountries();
          this.loading = false;
        },
        error: (error) => {
          console.error('Error deleting country:', error);
          this.messageService.showMessage({ type: 'error', text: 'Failed to delete country' });
          this.loading = false;
        }
      });
    }
  }

  resetForm(): void {
    this.countryForm.reset();
    this.isEditMode = false;
    this.currentCountryId = null;
  }

  // Helper method to get user identifier for creation/modification tracking
  private getUserIdentifier(): string {
    return this.currentUser ? this.currentUser.id || this.currentUser.email : 'system';
  }
}
