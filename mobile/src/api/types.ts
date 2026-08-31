export interface ApiResponse<T> {
  success: boolean;
  data: T;
  meta?: { traceId?: string | null; culture?: string | null };
}

export interface PagedList<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasNext: boolean;
}

export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  expiresAt: string;
}

export interface ProductListItem {
  id: string;
  sku: string;
  price: number;
  currency: string;
  isActive: boolean;
  name: string;
}
