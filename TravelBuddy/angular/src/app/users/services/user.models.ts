export interface CreateUserRequest {
  userName: string;
  email: string;
  password: string;
  confirmPassword: string;
  name: string;
  surname: string;
  phoneNumber?: string;
}

export interface LoginRequest {
  userNameOrEmailAddress: string;
  password: string;
  rememberMe?: boolean;
}

export interface LoginResponse {
  accessToken: string;
  refreshToken?: string;
  expiresIn: number;
}

export interface UpdateProfileRequest {
  name: string;
  surname: string;
  email: string;
  phoneNumber?: string;
  bio?: string;
}

export interface PreferencesResponse {
  theme: 'light' | 'dark';
  notifications: boolean;
  language: string;
}
