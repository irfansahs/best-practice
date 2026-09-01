import * as React from 'react';
import { Pressable, type PressableProps, ActivityIndicator } from 'react-native';
import { cva, type VariantProps } from 'class-variance-authority';
import { cn } from '@/lib/utils';
import { Text } from '@/components/ui/text';

const buttonVariants = cva(
  'flex-row items-center justify-center rounded-md px-4 min-h-[44px] active:opacity-80',
  {
    variants: {
      variant: {
        default: 'bg-primary',
        outline: 'border border-border bg-background',
        destructive: 'bg-destructive',
        ghost: 'bg-transparent',
      },
      size: {
        default: 'py-2.5',
        sm: 'py-1.5 px-3 min-h-[36px]',
        icon: 'h-9 w-9 p-0',
      },
    },
    defaultVariants: {
      variant: 'default',
      size: 'default',
    },
  },
);

const buttonTextVariants = cva('text-sm font-semibold', {
  variants: {
    variant: {
      default: 'text-primary-foreground',
      outline: 'text-foreground',
      destructive: 'text-destructive-foreground',
      ghost: 'text-primary',
    },
  },
  defaultVariants: {
    variant: 'default',
  },
});

export interface ButtonProps extends PressableProps, VariantProps<typeof buttonVariants> {
  loading?: boolean;
  children: React.ReactNode;
  className?: string;
  textClassName?: string;
}

export function Button({
  variant,
  size,
  loading,
  disabled,
  children,
  className,
  textClassName,
  ...props
}: ButtonProps) {
  return (
    <Pressable
      className={cn(buttonVariants({ variant, size }), disabled || loading ? 'opacity-50' : '', className)}
      disabled={disabled || loading}
      {...props}
    >
      {loading ? (
        <ActivityIndicator color={variant === 'outline' || variant === 'ghost' ? '#5b5bd6' : '#fafafa'} />
      ) : typeof children === 'string' ? (
        <Text className={cn(buttonTextVariants({ variant }), textClassName)}>{children}</Text>
      ) : (
        children
      )}
    </Pressable>
  );
}

export { buttonVariants };
