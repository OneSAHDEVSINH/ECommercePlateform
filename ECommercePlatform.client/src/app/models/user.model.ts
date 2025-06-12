import { Role } from './role.model';

export interface User {
  id?: string;
  firstName: string;
  lastName: string;
  email: string;
  password?: string;
  roles: Role[]; // Now using Role[] instead of string[]
  isActive: boolean;
  phoneNumber?: string;
  bio?: string;
  createdOn?: Date;
  modifiedOn?: Date;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  user: User;
}
