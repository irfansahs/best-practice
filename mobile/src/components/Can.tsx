import type { ReactNode } from 'react';
import { PermissionScope } from '@/api/types';
import { useAuth } from '@/contexts/AuthContext';

interface CanProps {
  permission: string;
  minScope?: number;
  children: ReactNode;
}

export function Can({ permission, minScope = PermissionScope.Organization, children }: CanProps) {
  const { hasPermission } = useAuth();
  if (!hasPermission(permission, minScope)) return null;
  return <>{children}</>;
}
