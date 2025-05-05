import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

export type ThemeType = 'classic' | 'modern';

@Injectable({
  providedIn: 'root'
})
export class ThemeService {
  private themeSubject = new BehaviorSubject<ThemeType>('classic');
  public theme$: Observable<ThemeType> = this.themeSubject.asObservable();

  constructor() {
    // Load saved theme from local storage if available
    const savedTheme = localStorage.getItem('adminTheme') as ThemeType;
    if (savedTheme) {
      this.themeSubject.next(savedTheme);
    }
  }

  getTheme(): ThemeType {
    return this.themeSubject.value;
  }

  setTheme(theme: ThemeType): void {
    this.themeSubject.next(theme);
    localStorage.setItem('adminTheme', theme);
  }

  toggleTheme(): void {
    const currentTheme = this.themeSubject.value;
    const newTheme: ThemeType = currentTheme === 'classic' ? 'modern' : 'classic';
    this.setTheme(newTheme);
  }
} 