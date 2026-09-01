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

export interface CurrentUserDto {
  id: string;
  email: string;
  fullName: string;
  permissions: string[];
}

export interface ProductListItem {
  id: string;
  sku: string;
  price: number;
  currency: string;
  isActive: boolean;
  name: string;
}

export interface ProductDetailDto {
  id: string;
  sku: string;
  price: number;
  currency: string;
  categoryId: string;
  languageId: string;
  isActive: boolean;
  name: string;
  description: string | null;
  slug: string;
}

export interface CategoryListItem {
  id: string;
  name: string;
  isActive: boolean;
  parentCategoryId: string | null;
}
