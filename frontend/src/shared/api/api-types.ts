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

export const PermissionScope = {
  Own: 0,
  Organization: 1,
  Subtree: 2,
  Global: 3,
} as const;

export type PermissionScopeValue = (typeof PermissionScope)[keyof typeof PermissionScope];

export interface OrganizationSummary {
  id: string;
  name: string;
  slug: string;
  type: string;
  path: string;
  isPrimary: boolean;
}

export interface CurrentUser {
  id: string;
  email: string;
  fullName: string;
  permissions: Record<string, number>;
  activeOrganization: OrganizationSummary | null;
  organizations: OrganizationSummary[];
  isImpersonating: boolean;
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

export interface OrganizationListItem {
  id: string;
  parentId: string | null;
  name: string;
  slug: string;
  type: string;
  status: string;
  path: string;
  depth: number;
}

export interface OrganizationDetail extends OrganizationListItem {
  contactEmail: string | null;
  timeZoneId: string;
  defaultCulture: string;
}

export interface MemberListItem {
  membershipId: string;
  userId: string;
  email: string;
  fullName: string;
  status: string;
  isPrimary: boolean;
  title: string | null;
  roles: string[];
}

export interface RolePermissionGrant {
  permissionId: string;
  code: string;
  scope: number;
}

export interface RoleListItem {
  id: string;
  name: string;
  description: string | null;
  isSystemRole: boolean;
  organizationId: string | null;
  allowedClients: number;
  permissions: RolePermissionGrant[];
}

export interface PermissionCatalogItem {
  id: string;
  code: string;
  description: string | null;
  module: string;
  maxScope: number;
  isPlatformOnly: boolean;
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
  Tenancy: {
    Organizations: {
      Read: 'tenancy.organizations.read',
      Create: 'tenancy.organizations.create',
      Update: 'tenancy.organizations.update',
      Delete: 'tenancy.organizations.delete',
      Impersonate: 'tenancy.organizations.impersonate',
    },
    Members: {
      Read: 'tenancy.members.read',
      Manage: 'tenancy.members.manage',
    },
    Roles: {
      Read: 'tenancy.roles.read',
      Manage: 'tenancy.roles.manage',
    },
    Permissions: {
      Read: 'tenancy.permissions.read',
    },
  },
} as const;
