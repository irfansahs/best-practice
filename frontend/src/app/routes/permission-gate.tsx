import type { ReactNode } from 'react';
import { PermissionScope } from '@/shared/api/api-types';
import { usePermission } from '@/features/auth/hooks/use-permission';
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
  const hasPermission = usePermission(permission, minScope);

  if (!hasPermission) {
    return <>{fallback ?? <AccessDenied />}</>;
  }

  return <>{children}</>;
}
