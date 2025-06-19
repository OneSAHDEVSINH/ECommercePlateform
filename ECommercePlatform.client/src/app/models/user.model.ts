import { Role } from './role.model';

export interface User {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  password?: string;
  roles: Role[];
  isActive: boolean;
  phoneNumber?: string;
  bio?: string;
  createdOn?: Date;
  modifiedOn?: Date;
}

export interface LoginResponse {
  token: string;
  user: User;
  permissions?: UserPermissionDto[];
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface UserPermissionDto {
  moduleName: string;
  canView: boolean;
  canAdd: boolean;
  canEdit: boolean;
  canDelete: boolean;
}
