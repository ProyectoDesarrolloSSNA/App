import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { ProfileComponent } from './profile.component';
import { UserService } from '../services/user.service';
import { of, throwError } from 'rxjs';

describe('ProfileComponent', () => {
  let component: ProfileComponent;
  let fixture: ComponentFixture<ProfileComponent>;
  let userService: jasmine.SpyObj<UserService>;

  beforeEach(async () => {
    const spy = jasmine.createSpyObj('UserService', ['getCurrentProfile', 'deleteAccount']);

    await TestBed.configureTestingModule({
      imports: [ProfileComponent, HttpClientTestingModule, RouterTestingModule],
      providers: [{ provide: UserService, useValue: spy }],
    }).compileComponents();

    userService = TestBed.inject(UserService) as jasmine.SpyObj<UserService>;
    fixture = TestBed.createComponent(ProfileComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load profile on init', () => {
    const mockProfile = {
      id: '1',
      userName: 'testuser',
      email: 'test@example.com',
      name: 'Test',
      surname: 'User',
      createdAt: new Date(),
    };

    userService.getCurrentProfile.and.returnValue(of(mockProfile));

    component.ngOnInit();

    expect(userService.getCurrentProfile).toHaveBeenCalled();
    expect(component.profile).toEqual(mockProfile);
    expect(component.isLoading).toBeFalse();
  });

  it('should handle profile load error', () => {
    userService.getCurrentProfile.and.returnValue(throwError(() => new Error('Error')));

    component.ngOnInit();

    expect(component.error).toBeTruthy();
    expect(component.isLoading).toBeFalse();
  });
});
