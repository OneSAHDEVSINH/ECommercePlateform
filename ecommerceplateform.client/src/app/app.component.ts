import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { FaviconService } from './services/favicon.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  constructor(private faviconService: FaviconService) { }
  title = 'E-Commerce Plateform 💲';
}
