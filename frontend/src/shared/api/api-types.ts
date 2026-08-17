export interface ApiMeta {
  traceId?: string | null;
  culture?: string | null;
}

export interface ApiResponse<T> {
  success: boolean;
  data: T;
  meta: ApiMeta;
}

export interface ApiError {
  code: string;
  message: string;
  errors?: Record<string, string[]>;
}

export interface PagedList<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasNext: boolean;
}

export interface CurrentUser {
  id: string;
  email: string;
  fullName: string;
  permissions: string[];
}

export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  expiresAt: string;
}

export interface RefreshTokenResponse {
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

export interface ProductDetail {
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

export interface CategoryDetail {
  id: string;
  parentCategoryId: string | null;
  isActive: boolean;
  languageId: string;
  name: string;
  description: string | null;
  slug: string;
}

export interface CreateCategoryResponse {
  id: string;
}

export interface CreateProductResponse {
  id: string;
  sku: string;
}

export interface Language {
  id: string;
  code: string;
  name: string;
  nativeName: string;
  isDefault: boolean;
  isActive: boolean;
  sortOrder: number;
}

export interface ResourceBundle {
  culture: string;
  resources: Record<string, string>;
}

export interface UpsertTranslationResponse {
  id: string;
}

export interface ImportTranslationsResponse {
  importedCount: number;
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
  Localization: {
    Read: 'localization.translations.read',
    Manage: 'localization.translations.manage',
  },
} as const;
