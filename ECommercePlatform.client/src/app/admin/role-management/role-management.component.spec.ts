import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { RoleManagementComponent } from './role-management.component';
import { RoleService } from '../../services/role/role.service';
import { AuthService } from '../../services/auth/auth.service';
import { MessageService } from '../../services/general/message.service';
import { of } from 'rxjs';

describe('RoleManagementComponent', () => {
  let component: RoleManagementComponent;
  let fixture: ComponentFixture<RoleManagementComponent>;

  // Create mock services
  const mockRoleService = {
    getPagedRoles: () => of({ items: [], totalCount: 0, totalPages: 0 }),
    getModules: () => of([])
  };

  const mockAuthService = {
    currentUser$: of(null)
  };

  const mockMessageService = {
    currentMessage: of(null),
    showMessage: () => { }
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        HttpClientTestingModule,
        ReactiveFormsModule,
        RoleManagementComponent
      ],
      providers: [
        { provide: RoleService, useValue: mockRoleService },
        { provide: AuthService, useValue: mockAuthService },
        { provide: MessageService, useValue: mockMessageService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(RoleManagementComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
