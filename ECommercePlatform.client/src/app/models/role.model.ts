export interface Module {
  id: string;
  name: string;
  description: string;
  route: string;
  icon: string;
  displayOrder: number;
  isActive: boolean;
}

export interface Permission {
  id: string;
  name: string;
  description: string;
  type: PermissionType;
  moduleId: string;
  moduleName?: string;
  isActive: boolean;
}

export enum PermissionType {
  VIEW = 'View',
  EDIT = 'Edit',
  DELETE = 'Delete',
  ADD = 'Add'
}

export interface Role {
  id?: string;
  name: string;
  description: string;
  isActive: boolean;
  permissions?: ModulePermission[];
  createdOn?: Date;
  createdBy?: string;
  modifiedOn?: Date;
  modifiedBy?: string;
}

export interface ModulePermission {
  moduleId: string;
  moduleName: string;
  canView: boolean;
  canCreate: boolean;
  canEdit: boolean;
  canDelete: boolean;
}

export interface RolePermissionRequest {
  roleId: string;
  permissions: {
    moduleId: string;
    permissionTypes: PermissionType[];
  }[];
}

export interface UserRoleAssignment {
  userId: string;
  roleIds: string[];
}
