import { test, expect } from '@playwright/test';

test.describe('Auth', () => {
  test('login page renders', async ({ page }) => {
    await page.goto('/login');
    await expect(page.getByRole('heading', { name: /sign in/i })).toBeVisible();
  });

  test('admin can login and see products', async ({ page }) => {
    await page.goto('/login');
    await page.getByLabel(/email/i).fill('admin@local.dev');
    await page.getByLabel(/password/i).fill('ChangeMe123!');
    await page.getByRole('button', { name: /sign in/i }).click();
    await expect(page).toHaveURL(/\/products/);
    await expect(page.getByRole('heading', { name: /products/i })).toBeVisible();
  });
});

test.describe('Catalog', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/login');
    await page.getByLabel(/email/i).fill('admin@local.dev');
    await page.getByLabel(/password/i).fill('ChangeMe123!');
    await page.getByRole('button', { name: /sign in/i }).click();
    await expect(page).toHaveURL(/\/products/);
  });

  test('categories page lists seeded category', async ({ page }) => {
    await page.goto('/categories');
    await expect(page.getByText('General')).toBeVisible();
  });

  test('can open new product form with dropdowns', async ({ page }) => {
    await page.goto('/products/new');
    await expect(page.getByText(/select a category/i)).toBeVisible();
    await expect(page.getByText(/select a language/i)).toBeVisible();
  });
});
