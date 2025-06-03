export interface PagedRequest {
  pageNumber: number;
  pageSize: number;
  searchText?: string;
  sortColumn?: string;
  sortDirection?: 'asc' | 'desc';
  filters?: { [key: string]: any };
}

export interface PagedResponse<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}
