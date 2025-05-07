import { Injectable, Inject, signal } from '@angular/core';
import { DOCUMENT } from '@angular/common';
import { Router, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class FaviconService {

  constructor(@Inject(DOCUMENT) private dom: any, private router: Router) {
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe((event: NavigationEnd) => {
      this.updateFavicon(event.url);
    });
  }

  updateFavicon(url: string) {
    let faviconPath: string;

    if (url === '/admin/login') {
      signal(faviconPath = 'login.png');
    } else if (url === '/admin/dashboard') {
      signal(faviconPath = 'dashboard.png');
    } else if(url === '/admin/countries') {
      signal(faviconPath = 'countries.png');
    } else if (url === '/admin/states') {
      signal(faviconPath = 'states.png');
    } else if (url === '/admin/cities') {
      signal(faviconPath = 'cities.png');
    } else {
      signal(faviconPath = 'favicon.ico'); // Default favicon
    }

    let link: HTMLLinkElement = this.dom.querySelector("link[rel*='icon']") || this.dom.createElement('link');
    link.type = 'image/x-icon';
    link.rel = 'shortcut icon';
    link.href = faviconPath;
    this.dom.doc.head.appendChild(link);
  }
}
