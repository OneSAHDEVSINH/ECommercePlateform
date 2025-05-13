export interface Country {
  id?: string;
  name: string;
  code: string;
  createdOn?: Date;
  createdBy?: string;
  modifiedOn?: Date;
  modifiedBy?: string;
  isActive?: boolean;
  isDeleted?: boolean;
} 
