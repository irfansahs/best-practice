import type { ReactNode } from 'react';
import { useAppSelector } from '@/app/hooks';
import { selectHasPermission } from '@/features/auth/slice/auth-slice';
import { AccessDenied } from '@/shared/components/access-denied';

interface PermissionGateProps {
  permission: string;
  children: ReactNode;
  fallback?: ReactNode;
}

export function PermissionGate({ permission, children, fallback }: PermissionGateProps) {
  const hasPermission = useAppSelector(selectHasPermission(permission));

  if (!hasPermission) {
    return <>{fallback ?? <AccessDenied />}</>;
  }

  return <>{children}</>;
}
