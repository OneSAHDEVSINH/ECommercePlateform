export interface Module {
  id?: string;
  name?: string;
  description?: string;
  route?: string;
  icon?: string;
  displayOrder?: number;
  isActive?: boolean;
  createdOn?: Date;
  createdBy?: string;
  modifiedOn?: Date;
  modifiedBy?: string;
  permissions?: Permission[];
}

export interface Permission {
  id?: string;
  name?: string;
  description?: string;
  type?: PermissionType;
  moduleId?: string;
  moduleName?: string;
  isActive?: boolean;
}

export enum PermissionType {
  View = 'View',
  //Add = 'Add',
  //Edit = 'Edit',
  AddEdit = 'AddEdit',
  Delete = 'Delete'
}

export interface Role {
  id?: string;
  name?: string;
  description?: string;
  isActive?: boolean;
  permissions?: RolePermission[];
  createdOn?: Date;
  createdBy?: string;
  modifiedOn?: Date;
  modifiedBy?: string;
}

export interface ModulePermission {
  moduleId: string;
  moduleName: string;
  canView: boolean;
  //canCreate: boolean;
  //canEdit: boolean;
  canAddEdit: boolean;
  canDelete: boolean;
}

export interface RolePermission {
  id?: string;
  roleId?: string;
  permissionId?: string;
  permissionType?: string;
  moduleId?: string;
  moduleName?: string;
  moduleRoute?: string;
  isActive?: boolean;
}

export interface RolePermissionRequest {
  roleId: string;
  permissions: {
    moduleId: string;
    permissionTypes: string[];
  }[];
}

export interface UserRoleAssignment {
  userId: string;
  roleIds: string[];
}

export interface PermissionItem {
  moduleId: string;
  permissionType: string;
}
