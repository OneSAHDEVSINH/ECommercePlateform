export interface City {
  id?: string;
  name: string;
  stateId: string;
  state?: string;
  createdOn?: Date;
  createdBy?: string;
  modifiedOn?: Date;
  modifiedBy?: string;
  isActive?: boolean;
  isDeleted?: boolean;
} 