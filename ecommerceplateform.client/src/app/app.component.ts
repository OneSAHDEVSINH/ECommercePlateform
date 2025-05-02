import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: true,
  imports: [RouterOutlet, CommonModule],
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'ecommerceplateform.client';
}

