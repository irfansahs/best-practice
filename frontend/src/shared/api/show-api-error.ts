import type { TFunction } from 'i18next';
import { toast } from 'sonner';
import { getProblemMessage, getValidationErrors, isProblemDetails } from './problem-details';

export function showApiError(error: unknown, t: TFunction): void {
  if (error && typeof error === 'object' && 'data' in error) {
    const data = (error as { data: unknown }).data;
    if (isProblemDetails(data)) {
      // 403 toast is handled once by the axios response interceptor.
      if (data.status === 403) {
        return;
      }

      const validationErrors = getValidationErrors(data);
      if (validationErrors) {
        const first = Object.values(validationErrors)[0]?.[0];
        toast.error(first ? t(first) : getProblemMessage(data, t));
        return;
      }

      toast.error(getProblemMessage(data, t));
      return;
    }
  }

  toast.error(t('Common.Error'));
}
