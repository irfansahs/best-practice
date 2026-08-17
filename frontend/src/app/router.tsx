import { createBrowserRouter, Navigate } from 'react-router';
import { AppShell } from '@/shared/components/layout/app-shell';
import { ProtectedRoute } from '@/app/routes/protected-route';
import { GuestRoute } from '@/app/routes/guest-route';
import { LoginPage } from '@/features/auth/pages/login-page';
import { ProductsListPage } from '@/features/catalog/products/pages/products-list-page';
import { ProductFormPage } from '@/features/catalog/products/pages/product-form-page';
import { CategoriesListPage } from '@/features/catalog/categories/pages/categories-list-page';
import { CategoryFormPage } from '@/features/catalog/categories/pages/category-form-page';
import { TranslationManagerPage } from '@/features/localization/pages/translation-manager-page';
import { Permissions } from '@/shared/api/api-types';
import { PermissionGate } from '@/app/routes/permission-gate';

export const router = createBrowserRouter([
  {
    path: '/login',
    element: (
      <GuestRoute>
        <LoginPage />
      </GuestRoute>
    ),
  },
  {
    path: '/',
    element: (
      <ProtectedRoute>
        <AppShell />
      </ProtectedRoute>
    ),
    children: [
      { index: true, element: <Navigate to="/products" replace /> },
      {
        path: 'products',
        element: (
          <PermissionGate permission={Permissions.Catalog.Products.Read}>
            <ProductsListPage />
          </PermissionGate>
        ),
      },
      {
        path: 'products/new',
        element: (
          <PermissionGate permission={Permissions.Catalog.Products.Create}>
            <ProductFormPage />
          </PermissionGate>
        ),
      },
      {
        path: 'products/:id/edit',
        element: (
          <PermissionGate permission={Permissions.Catalog.Products.Update}>
            <ProductFormPage />
          </PermissionGate>
        ),
      },
      {
        path: 'categories',
        element: (
          <PermissionGate permission={Permissions.Catalog.Categories.Read}>
            <CategoriesListPage />
          </PermissionGate>
        ),
      },
      {
        path: 'categories/new',
        element: (
          <PermissionGate permission={Permissions.Catalog.Categories.Create}>
            <CategoryFormPage />
          </PermissionGate>
        ),
      },
      {
        path: 'categories/:id/edit',
        element: (
          <PermissionGate permission={Permissions.Catalog.Categories.Update}>
            <CategoryFormPage />
          </PermissionGate>
        ),
      },
      {
        path: 'localization',
        element: (
          <PermissionGate permission={Permissions.Localization.Manage}>
            <TranslationManagerPage />
          </PermissionGate>
        ),
      },
    ],
  },
  { path: '*', element: <Navigate to="/" replace /> },
]);
