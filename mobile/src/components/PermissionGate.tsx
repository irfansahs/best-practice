import type { ReactNode } from 'react';
import { View } from 'react-native';
import { PermissionScope } from '@/api/types';
import { useAuth } from '@/contexts/AuthContext';
import { Text } from '@/components/ui/text';

interface PermissionGateProps {
  permission: string;
  minScope?: number;
  children: ReactNode;
  message?: string;
}

export function PermissionGate({
  permission,
  minScope = PermissionScope.Organization,
  children,
  message = 'You do not have permission to view this screen.',
}: PermissionGateProps) {
  const { hasPermission } = useAuth();

  if (!hasPermission(permission, minScope)) {
    return (
      <View className="flex-1 items-center justify-center bg-background px-4">
        <Text variant="muted" className="text-center">
          {message}
        </Text>
      </View>
    );
  }

  return <>{children}</>;
}
