import { Component, OnInit } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth/auth.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
  standalone: true,
  imports: [CommonModule, RouterModule]
})
export class DashboardComponent implements OnInit {
  userName: string = '';

  constructor(
    private authService: AuthService,
    private router: Router
  ) { }

  ngOnInit(): void {
    // The user info will now be displayed in the main AdminLTE navbar
    // You don't need to handle it here unless you want to show it in the content area
    this.authService.currentUser$.subscribe(user => {
      if (user) {
        const firstName = user.firstName || user['firstName'] || '';
        const lastName = user.lastName || user['lastName'] || '';
        this.userName = `${firstName} ${lastName}`.trim() || user.email;
      }
    });
  }
}





//import { Component, OnInit } from '@angular/core';
//import { Router, RouterModule } from '@angular/router';
//import { CommonModule } from '@angular/common';
//import { AuthService } from '../../services/auth/auth.service';
//import { NavbarComponent } from '../navbar/navbar.component';

//@Component({
//  selector: 'app-dashboard',
//  templateUrl: './dashboard.component.html',
//  styleUrls: ['./dashboard.component.scss'],
//  standalone: true,
//  imports: [CommonModule, RouterModule, NavbarComponent]
//})
//export class DashboardComponent implements OnInit {
//  userName: string = '';

//  constructor(
//    private authService: AuthService,
//    private router: Router
//  ) { }

//  ngOnInit(): void {
//    this.authService.currentUser$.subscribe(user => {
//      console.log('Current user in dashboard:', user);
//      if (user) {
//        const firstName = user.firstName || user['firstName'] || '';
//        const lastName = user.lastName || user['lastName'] || '';
//        this.userName = `${firstName} ${lastName}`.trim() || user.email;
//        console.log('User name set to:', this.userName);
//      }
//    });
//  }

//  logout(): void {
//    this.authService.logout();
//    this.router.navigate(['/admin/login']);
//  }
//}
