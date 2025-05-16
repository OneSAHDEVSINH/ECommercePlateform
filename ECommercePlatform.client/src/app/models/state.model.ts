export interface State {
  id?: string;
  name: string;
  code: string;
  countryId: string;
  country?: string;
  createdOn?: Date;
  createdBy?: string;
  modifiedOn?: Date;
  modifiedBy?: string;
  isActive?: boolean;
  isDeleted?: boolean;
} 