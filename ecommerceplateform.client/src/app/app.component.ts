// src/app/app.component.ts
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { FaviconService } from './services/general/favicon.service';
import { MessageComponent } from './shared/message/message.component';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
  standalone: true
})
export class AppComponent {
  constructor(private faviconService: FaviconService) { }
  title = 'E-Commerce Plateform 💲';
}
