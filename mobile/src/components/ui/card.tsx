import * as React from 'react';
import { View, type ViewProps } from 'react-native';
import { cn } from '@/lib/utils';
import { Text } from '@/components/ui/text';

export function Card({ className, ...props }: ViewProps & { className?: string }) {
  return <View className={cn('rounded-lg border border-border bg-card', className)} {...props} />;
}

export function CardHeader({ className, ...props }: ViewProps & { className?: string }) {
  return <View className={cn('flex flex-col gap-1.5 p-4', className)} {...props} />;
}

export function CardTitle({ className, children, ...props }: React.ComponentProps<typeof Text>) {
  return (
    <Text variant="h3" className={cn('font-semibold', className)} {...props}>
      {children}
    </Text>
  );
}

export function CardDescription({ className, children, ...props }: React.ComponentProps<typeof Text>) {
  return (
    <Text variant="muted" className={className} {...props}>
      {children}
    </Text>
  );
}

export function CardContent({ className, ...props }: ViewProps & { className?: string }) {
  return <View className={cn('p-4 pt-0', className)} {...props} />;
}

export function CardFooter({ className, ...props }: ViewProps & { className?: string }) {
  return <View className={cn('flex flex-row items-center p-4 pt-0', className)} {...props} />;
}
