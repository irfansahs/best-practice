import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import { initI18n } from '@/shared/i18n';
import { AppProviders } from '@/app/providers';
import '@/styles/globals.css';

async function bootstrap() {
  await initI18n();

  const root = document.getElementById('root');
  if (!root) throw new Error('Root element not found');

  createRoot(root).render(
    <StrictMode>
      <AppProviders />
    </StrictMode>,
  );
}

void bootstrap();
