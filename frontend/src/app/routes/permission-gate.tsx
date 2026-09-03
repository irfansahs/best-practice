import type { ReactNode } from 'react';
import { PermissionScope } from '@/shared/api/api-types';
import { useAppSelector } from '@/app/hooks';
import { selectHasPermission } from '@/features/auth/slice/auth-slice';
import { AccessDenied } from '@/shared/components/access-denied';

interface PermissionGateProps {
  permission: string;
  minScope?: number;
  children: ReactNode;
  fallback?: ReactNode;
}

export function PermissionGate({
  permission,
  minScope = PermissionScope.Organization,
  children,
  fallback,
}: PermissionGateProps) {
  const hasPermission = useAppSelector(selectHasPermission(permission, minScope));

  if (!hasPermission) {
    return <>{fallback ?? <AccessDenied />}</>;
  }

  return <>{children}</>;
}
