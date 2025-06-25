import { Role } from './role.model';

export interface User {
  id?: string;
  firstName: string;
  lastName: string;
  email: string;
  password?: string;
  roles: Role[];
  isActive: boolean;
  phoneNumber?: string;
  gender?: Gender | string;
  dateOfBirth?: Date | string;
  bio?: string;
  createdOn?: Date;
  modifiedOn?: Date;
  claims?: any[];
}

export enum Gender {
  Male = 0,
  Female = 1,
  Other = 2
}

// Helper functions for consistent conversion
export const genderToString = (gender: Gender | number | string | undefined): string => {
  if (gender === undefined || gender === null) return '';

  switch (Number(gender)) {
    case Gender.Male: return 'Male';
    case Gender.Female: return 'Female';
    case Gender.Other: return 'Other';
    default: return '';
  }
};

export const stringToGender = (gender: string | undefined): Gender => {
  if (!gender) return Gender.Other;

  switch (gender.toLowerCase()) {
    case 'male': return Gender.Male;
    case 'female': return Gender.Female;
    default: return Gender.Other;
  }
};

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
  canAddEdit: boolean;
  canDelete: boolean;
}
