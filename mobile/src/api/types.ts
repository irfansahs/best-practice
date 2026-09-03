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

export const PermissionScope = {
  Own: 0,
  Organization: 1,
  Subtree: 2,
  Global: 3,
} as const;

export interface OrganizationSummary {
  id: string;
  name: string;
  slug: string;
  type: string;
  path: string;
  isPrimary: boolean;
}

export interface CurrentUserDto {
  id: string;
  email: string;
  fullName: string;
  permissions: Record<string, number>;
  activeOrganization: OrganizationSummary | null;
  organizations: OrganizationSummary[];
  isImpersonating: boolean;
}

export const Permissions = {
  Catalog: {
    Products: {
      Read: 'catalog.products.read',
      Create: 'catalog.products.create',
      Update: 'catalog.products.update',
      Delete: 'catalog.products.delete',
    },
    Categories: {
      Read: 'catalog.categories.read',
      Create: 'catalog.categories.create',
      Update: 'catalog.categories.update',
      Delete: 'catalog.categories.delete',
    },
  },
} as const;

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
