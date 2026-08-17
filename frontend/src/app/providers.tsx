import { Provider } from 'react-redux';
import { RouterProvider } from 'react-router';
import { I18nextProvider } from 'react-i18next';
import { store } from '@/app/store';
import { router } from '@/app/router';
import { AuthBootstrap } from '@/app/auth-bootstrap';
import { Toaster } from '@/shared/components/ui/sonner';
import { i18n } from '@/shared/i18n';

export function AppProviders() {
  return (
    <Provider store={store}>
      <I18nextProvider i18n={i18n}>
        <AuthBootstrap>
          <RouterProvider router={router} />
          <Toaster richColors position="top-right" />
        </AuthBootstrap>
      </I18nextProvider>
    </Provider>
  );
}
