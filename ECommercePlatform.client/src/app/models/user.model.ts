export enum UserRole {
  Admin = 'Admin',
  Customer = 'Customer',
  Vendor = 'Vendor',
  Seller = 'Seller',
  Guest = 'Guest'
}

export interface User {
  id?: string;
  firstName: string;
  lastName: string;
  email: string;
  password?: string;
  role: UserRole;
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
