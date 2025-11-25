import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { ChangePasswordComponent } from './change-password.component';
import { UserService } from '../services/user.service';
import { of, throwError } from 'rxjs';

describe('ChangePasswordComponent', () => {
  let component: ChangePasswordComponent;
  let fixture: ComponentFixture<ChangePasswordComponent>;
  let userService: jasmine.SpyObj<UserService>;

  beforeEach(async () => {
    const spy = jasmine.createSpyObj('UserService', ['changePassword']);

    await TestBed.configureTestingModule({
      imports: [ChangePasswordComponent, HttpClientTestingModule, RouterTestingModule, ReactiveFormsModule],
      providers: [{ provide: UserService, useValue: spy }],
    }).compileComponents();

    userService = TestBed.inject(UserService) as jasmine.SpyObj<UserService>;
    fixture = TestBed.createComponent(ChangePasswordComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should validate password match', () => {
    component.changePasswordForm.patchValue({
      currentPassword: 'oldpass123',
      newPassword: 'newpass123',
      confirmPassword: 'differentpass',
    });

    expect(component.changePasswordForm.hasError('passwordMismatch')).toBeTruthy();
  });

  it('should validate minimum password length', () => {
    component.changePasswordForm.patchValue({
      newPassword: 'short',
    });

    expect(component.changePasswordForm.get('newPassword')?.hasError('minlength')).toBeTruthy();
  });

  it('should call changePassword service on submit', () => {
    userService.changePassword.and.returnValue(of(void 0));

    component.changePasswordForm.patchValue({
      currentPassword: 'oldpass123',
      newPassword: 'newpass123',
      confirmPassword: 'newpass123',
    });

    component.changePassword();

    expect(userService.changePassword).toHaveBeenCalled();
    expect(component.successMessage).toBeTruthy();
  });
});
