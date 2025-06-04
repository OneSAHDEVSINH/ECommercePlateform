export interface PagedRequest {
  pageNumber: number;
  pageSize: number;
  searchText?: string;
  sortColumn?: string;
  sortDirection?: 'asc' | 'desc';
  filters?: { [key: string]: any };
  startDate?: string | null; // ISO format date string
  endDate?: string | null;   // ISO format date string
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
