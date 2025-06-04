import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { DateFilterService } from '../../services/general/date-filter.service';

@Component({
  selector: 'app-date-range-filter',
  templateUrl: './date-range-filter.component.html',
  styleUrls: ['./date-range-filter.component.scss'],
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule]
})
export class DateRangeFilterComponent implements OnInit {
  dateRangeForm!: FormGroup;

  constructor(
    private fb: FormBuilder,
    private dateFilterService: DateFilterService
  ) { }

  ngOnInit(): void {
    this.dateRangeForm = this.fb.group({
      startDate: [null],
      endDate: [null]
    });

    this.dateRangeForm.valueChanges.subscribe(value => {
      // Only trigger filter when at least one date is set
      if (value.startDate || value.endDate) {
        // Convert string values from datetime-local inputs to Date objects
        const startDate = value.startDate ? new Date(value.startDate) : null;
        const endDate = value.endDate ? new Date(value.endDate) : null;

        // Validate dates before sending
        if ((startDate && isNaN(startDate.getTime())) ||
          (endDate && isNaN(endDate.getTime()))) {
          console.error('Invalid date value');
          return;
        }

        // Ensure end date is after start date if both are provided
        if (startDate && endDate && startDate > endDate) {
          console.error('Start date cannot be after end date');
          return;
        }

        this.dateFilterService.setDateRange(startDate, endDate);
      }
    });
  }

  clearDateRange(): void {
    this.dateRangeForm.reset();
    this.dateFilterService.clearDateRange();
  }
}
