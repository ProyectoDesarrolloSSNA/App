import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { PublicProfileComponent } from './public-profile.component';
import { UserService } from '../services/user.service';
import { of, throwError } from 'rxjs';

describe('PublicProfileComponent', () => {
  let component: PublicProfileComponent;
  let fixture: ComponentFixture<PublicProfileComponent>;
  let userService: jasmine.SpyObj<UserService>;

  beforeEach(async () => {
    const spy = jasmine.createSpyObj('UserService', ['getPublicProfile']);

    await TestBed.configureTestingModule({
      imports: [PublicProfileComponent, HttpClientTestingModule, RouterTestingModule],
      providers: [{ provide: UserService, useValue: spy }],
    }).compileComponents();

    userService = TestBed.inject(UserService) as jasmine.SpyObj<UserService>;
    fixture = TestBed.createComponent(PublicProfileComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load public profile when route params change', () => {
    const mockProfile = {
      id: '123',
      userName: 'publicuser',
      email: 'public@example.com',
      name: 'Public',
      surname: 'User',
      bio: 'I am a public user',
      createdAt: new Date(),
    };

    userService.getPublicProfile.and.returnValue(of(mockProfile));

    component.ngOnInit();

    expect(component.profile).toEqual(mockProfile);
    expect(component.isLoading).toBeFalse();
  });

  it('should handle profile load error', () => {
    userService.getPublicProfile.and.returnValue(throwError(() => new Error('Not found')));

    component.ngOnInit();

    expect(component.error).toBeTruthy();
  });
});
