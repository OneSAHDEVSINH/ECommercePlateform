import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, debounceTime } from 'rxjs';

export interface DateRange {
  startDate: Date | null;
  endDate: Date | null;
}

@Injectable({
  providedIn: 'root'
})
export class DateFilterService {
  private dateRangeSubject = new BehaviorSubject<DateRange>({ startDate: null, endDate: null });

  setDateRange(startDate: Date | null, endDate: Date | null): void {
    if (startDate) {
      // Set time to start of day for consistent filtering
      startDate.setHours(0, 0, 0, 0);
    }
    if (endDate) {
      // Set time to end of day for consistent filtering
      endDate.setHours(23, 59, 59, 999);
    }
    this.dateRangeSubject.next({ startDate, endDate });
  }

  clearDateRange(): void {
    this.dateRangeSubject.next({ startDate: null, endDate: null });
  }

  getDateRangeObservable(): Observable<DateRange> {
    return this.dateRangeSubject.asObservable().pipe(
      debounceTime(300) // Prevent too many API calls
    );
  }
}
