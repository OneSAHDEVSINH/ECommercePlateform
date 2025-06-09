export interface User {
  id?: string;
  firstName: string;
  lastName: string;
  email: string;
  password?: string;
  //role: UserRole;
  roles: string[]; //Role[] for more info
  isActive: boolean;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  user: User;
} 
