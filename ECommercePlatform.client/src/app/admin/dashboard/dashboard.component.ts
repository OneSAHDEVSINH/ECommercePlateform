import { Component, OnInit } from '@angular/core';
import { Router, RouterModule, ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth/auth.service';
import { PermissionDirective } from '../../directives/permission.directive';
import { PermissionType } from '../../models/role.model';
import { AuthorizationService } from '../../services/authorization/authorization.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
  standalone: true,
  imports: [CommonModule, RouterModule]
})
export class DashboardComponent implements OnInit {
  userName: string = '';
  accessDeniedMessage: string | null = null;
  PermissionType = PermissionType;

  constructor(
    private authService: AuthService,
    public authorizationService: AuthorizationService,
    private router: Router,
    private route: ActivatedRoute
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
