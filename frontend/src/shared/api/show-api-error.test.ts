import { describe, expect, it, vi } from 'vitest';
import { showApiError } from './show-api-error';

vi.mock('sonner', () => ({
  toast: {
    error: vi.fn(),
  },
}));

import { toast } from 'sonner';

describe('showApiError', () => {
  beforeEach(() => {
    vi.mocked(toast.error).mockClear();
  });

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

  it('skips toast for 403 (handled by axios interceptor)', () => {
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
    expect(toast.error).not.toHaveBeenCalled();
  });
});
