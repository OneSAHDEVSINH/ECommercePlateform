//import { Injectable } from '@angular/core';
//import { BehaviorSubject, Observable } from 'rxjs';

//export type ThemeType = 'classic' | 'modern';

//@Injectable({
//  providedIn: 'root'
//})
//export class ThemeService {
//  private themeSubject = new BehaviorSubject<ThemeType>('classic');
//  public theme$: Observable<ThemeType> = this.themeSubject.asObservable();

//  constructor() {
//    // Load saved theme from local storage if available
//    const savedTheme = localStorage.getItem('adminTheme') as ThemeType;
//    if (savedTheme) {
//      this.themeSubject.next(savedTheme);
//    }
//  }

//  getTheme(): ThemeType {
//    return this.themeSubject.value;
//  }

//  setTheme(theme: ThemeType): void {
//    this.themeSubject.next(theme);
//    localStorage.setItem('adminTheme', theme);
//  }

//  toggleTheme(): void {
//    const currentTheme = this.themeSubject.value;
//    const newTheme: ThemeType = currentTheme === 'classic' ? 'modern' : 'classic';
//    this.setTheme(newTheme);
//  }
//}

import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

export type ThemeType = 'classic' | 'modern';

@Injectable({
  providedIn: 'root'
})
export class ThemeService {
  private themeSubject = new BehaviorSubject<ThemeType>(this.getInitialTheme());
  theme$ = this.themeSubject.asObservable();

  constructor() {
    // Check if a theme is stored in localStorage
    this.loadSavedTheme();

    // Watch for system preference changes
    this.watchSystemPreference();
  }

  private getInitialTheme(): ThemeType {
    const savedTheme = localStorage.getItem('app-theme') as ThemeType;
    if (savedTheme && (savedTheme === 'classic' || savedTheme === 'modern')) {
      return savedTheme;
    }

    // Use system preference as fallback
    return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'modern' : 'classic';
  }

  private loadSavedTheme(): void {
    const savedTheme = localStorage.getItem('app-theme') as ThemeType;
    if (savedTheme && (savedTheme === 'classic' || savedTheme === 'modern')) {
      this.setTheme(savedTheme);
    }
  }

  private watchSystemPreference(): void {
    // Only apply system preference if no theme is explicitly set
    window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', e => {
      if (!localStorage.getItem('app-theme')) {
        this.setTheme(e.matches ? 'modern' : 'classic');
      }
    });
  }

  getCurrentTheme(): ThemeType {
    return this.themeSubject.value;
  }

  setTheme(theme: ThemeType): void {
    localStorage.setItem('app-theme', theme);
    this.themeSubject.next(theme);

    // Update document class for global styling
    document.documentElement.classList.remove('theme-classic', 'theme-modern');
    document.documentElement.classList.add(`theme-${theme}`);
  }

  toggleTheme(): void {
    const newTheme = this.themeSubject.value === 'classic' ? 'modern' : 'classic';
    this.setTheme(newTheme);
  }
}
