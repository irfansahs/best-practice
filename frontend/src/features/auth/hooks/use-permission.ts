import { PermissionScope } from '@/shared/api/api-types';
import { useAppSelector } from '@/app/hooks';
import { selectHasPermission } from '@/features/auth/slice/auth-slice';

export function usePermission(permission: string, minScope: number = PermissionScope.Organization) {
  return useAppSelector(selectHasPermission(permission, minScope));
}
