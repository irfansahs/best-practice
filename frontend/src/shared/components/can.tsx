import type { ReactNode } from 'react';
import { PermissionScope } from '@/shared/api/api-types';
import { PermissionGate } from '@/app/routes/permission-gate';

interface CanProps {
  permission: string;
  minScope?: number;
  children: ReactNode;
  fallback?: ReactNode;
}

export function Can({
  permission,
  minScope = PermissionScope.Organization,
  children,
  fallback = null,
}: CanProps) {
  return (
    <PermissionGate permission={permission} minScope={minScope} fallback={fallback}>
      {children}
    </PermissionGate>
  );
}
