import * as React from 'react';
import { Text as RNText, type TextProps } from 'react-native';
import { cva, type VariantProps } from 'class-variance-authority';
import { cn } from '@/lib/utils';

const textVariants = cva('text-foreground', {
  variants: {
    variant: {
      h1: 'text-3xl font-bold',
      h2: 'text-2xl font-semibold',
      h3: 'text-lg font-semibold',
      p: 'text-base',
      muted: 'text-sm text-muted-foreground',
      label: 'text-sm font-medium',
      error: 'text-sm text-destructive',
    },
  },
  defaultVariants: {
    variant: 'p',
  },
});

export interface AppTextProps extends TextProps, VariantProps<typeof textVariants> {
  className?: string;
}

export function Text({ variant, className, ...props }: AppTextProps) {
  return <RNText className={cn(textVariants({ variant }), className)} {...props} />;
}
