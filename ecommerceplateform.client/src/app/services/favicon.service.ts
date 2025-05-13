import { Injectable, Inject } from '@angular/core';
import { DOCUMENT } from '@angular/common';
import { Router, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class FaviconService {

  private faviconPath = '';

  constructor(@Inject(DOCUMENT) private document: Document, private router: Router) {
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe((event: NavigationEnd) => {
      this.updateFavicon(event.url);
    });
  }

  updateFavicon(url: string) {
    //let faviconPath: string;

    if (url === '/admin/login') {
      this.faviconPath = 'login.png';
    } else if (url === '/admin/dashboard') {
      this.faviconPath = 'dashboard.png';
    } else if (url === '/admin/countries') {
      this.faviconPath = 'countries.png';
    } else if (url === '/admin/states') {
      this.faviconPath = 'states.png';
    } else if (url === '/admin/cities') {
      this.faviconPath = 'cities.png';
    } else {
      this.faviconPath = 'favicon.ico'; // Default favicon
    }

    let link: HTMLLinkElement = this.document.querySelector("link[rel*='icon']") as HTMLLinkElement ||
      this.document.createElement('link');
    link.type = 'image/x-icon';
    link.rel = 'shortcut icon';
    link.href = this.faviconPath;

    if (!this.document.querySelector("link[rel*='icon']")) {
      this.document.head.appendChild(link);
    }
  }
}
