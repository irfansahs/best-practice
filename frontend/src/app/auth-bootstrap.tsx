import { useEffect } from 'react';
import { useTranslation } from 'react-i18next';
import { useAppDispatch, useAppSelector } from '@/app/hooks';
import { bootstrapAuth, forceSessionExpired, selectAuthStatus } from '@/features/auth/slice/auth-slice';
import { setSessionExpiredHandler } from '@/shared/api/axios-base-query';

interface AuthBootstrapProps {
  children: React.ReactNode;
}

export function AuthBootstrap({ children }: AuthBootstrapProps) {
  const dispatch = useAppDispatch();
  const status = useAppSelector(selectAuthStatus);
  const { t } = useTranslation();

  useEffect(() => {
    setSessionExpiredHandler(() => {
      void dispatch(forceSessionExpired());
    });

    return () => setSessionExpiredHandler(null);
  }, [dispatch]);

  useEffect(() => {
    if (status === 'idle') {
      void dispatch(bootstrapAuth());
    }
  }, [dispatch, status]);

  if (status === 'idle' || status === 'bootstrapping') {
    return (
      <div className="flex min-h-screen items-center justify-center">
        <p className="text-muted-foreground">{t('Common.Loading')}</p>
      </div>
    );
  }

  return <>{children}</>;
}
