import { describe, expect, it } from 'vitest';
import { getProblemMessage, getValidationErrors, isProblemDetails } from '@/shared/api/problem-details';

describe('problem-details', () => {
  it('detects problem details payloads', () => {
    expect(isProblemDetails({ title: 'Error', status: 400 })).toBe(true);
    expect(isProblemDetails({ foo: 'bar' })).toBe(false);
  });

  it('prefers extensions.code via translator', () => {
    const message = getProblemMessage(
      {
        title: 'Not Found',
        status: 404,
        extensions: { code: 'Catalog.Product.NotFound' },
      },
      (key) => key,
    );

    expect(message).toBe('Catalog.Product.NotFound');
  });

  it('falls back to detail when no code', () => {
    expect(getProblemMessage({ detail: 'Invalid request' })).toBe('Invalid request');
    expect(getProblemMessage({ title: 'Bad Request' })).toBe('Bad Request');
  });

  it('extracts validation errors', () => {
    const errors = getValidationErrors({
      title: 'Validation Error',
      errors: { Sku: ['Catalog.Sku.Required'] },
    });

    expect(errors?.Sku?.[0]).toBe('Catalog.Sku.Required');
  });
});
