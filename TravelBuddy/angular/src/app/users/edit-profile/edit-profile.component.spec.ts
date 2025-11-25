import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { EditProfileComponent } from './edit-profile.component';
import { UserService } from '../services/user.service';
import { of, throwError } from 'rxjs';

describe('EditProfileComponent', () => {
  let component: EditProfileComponent;
  let fixture: ComponentFixture<EditProfileComponent>;
  let userService: jasmine.SpyObj<UserService>;

  beforeEach(async () => {
    const spy = jasmine.createSpyObj('UserService', [
      'getCurrentProfile',
      'updateProfile',
      'uploadProfilePicture',
    ]);

    await TestBed.configureTestingModule({
      imports: [EditProfileComponent, HttpClientTestingModule, RouterTestingModule, ReactiveFormsModule],
      providers: [{ provide: UserService, useValue: spy }],
    }).compileComponents();

    userService = TestBed.inject(UserService) as jasmine.SpyObj<UserService>;
    fixture = TestBed.createComponent(EditProfileComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load profile and populate form', () => {
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

    expect(component.profile).toEqual(mockProfile);
    expect(component.editForm.get('name')?.value).toBe('Test');
    expect(component.editForm.get('email')?.value).toBe('test@example.com');
  });

  it('should validate form fields', () => {
    component.editForm.get('name')?.setValue('');
    expect(component.editForm.get('name')?.hasError('required')).toBeTruthy();

    component.editForm.get('email')?.setValue('invalid-email');
    expect(component.editForm.get('email')?.hasError('email')).toBeTruthy();
  });
});
