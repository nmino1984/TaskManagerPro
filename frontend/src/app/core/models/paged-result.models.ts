export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages?: number;
  hasNextPage?: boolean;
  hasPreviousPage?: boolean;
}

export interface TaskQueryParams {
  page: number;
  pageSize: number;
  status?: string;
  priority?: string;
  search?: string;
}
