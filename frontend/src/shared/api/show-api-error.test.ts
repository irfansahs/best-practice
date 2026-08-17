import { describe, expect, it, vi } from 'vitest';
import { showApiError } from './show-api-error';

vi.mock('sonner', () => ({
  toast: {
    error: vi.fn(),
  },
}));

import { toast } from 'sonner';

describe('showApiError', () => {
  it('uses extensions.code for domain errors', () => {
    const t = (key: string) => key;
    showApiError(
      {
        data: {
          title: 'Not Found',
          status: 404,
          extensions: { code: 'Catalog.Product.NotFound' },
        },
      },
      t,
    );

    expect(toast.error).toHaveBeenCalledWith('Catalog.Product.NotFound');
  });

  it('maps 403 to access denied title', () => {
    const t = (key: string) => key;
    showApiError(
      {
        data: {
          title: 'Forbidden',
          status: 403,
        },
      },
      t,
    );

    expect(toast.error).toHaveBeenCalledWith('Common.AccessDenied.Title');
  });
});
