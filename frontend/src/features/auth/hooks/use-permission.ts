import { PermissionScope } from '@/shared/api/api-types';
import { useAppSelector } from '@/app/hooks';
import { selectPermissions } from '@/features/auth/slice/auth-slice';

export function usePermission(permission: string, minScope: number = PermissionScope.Organization) {
  const permissions = useAppSelector(selectPermissions);
  return (permissions[permission] ?? -1) >= minScope;
}
