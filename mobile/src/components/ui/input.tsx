import * as React from 'react';
import { View, TextInput, type TextInputProps, Pressable } from 'react-native';
import { cn } from '@/lib/utils';
import { Text } from '@/components/ui/text';

export interface InputProps extends TextInputProps {
  label?: string;
  error?: string;
  containerClassName?: string;
  className?: string;
  secureToggle?: boolean;
}

export function Input({
  label,
  error,
  containerClassName,
  className,
  secureTextEntry,
  secureToggle,
  ...props
}: InputProps) {
  const [secure, setSecure] = React.useState(secureTextEntry ?? false);

  React.useEffect(() => {
    if (secureTextEntry !== undefined) setSecure(secureTextEntry);
  }, [secureTextEntry]);

  return (
    <View className={cn('gap-1.5', containerClassName)}>
      {label ? <Text variant="label">{label}</Text> : null}
      <View className="relative">
        <TextInput
          className={cn(
            'rounded-md border border-input bg-background px-3 py-2.5 text-base text-foreground min-h-[44px]',
            error ? 'border-destructive' : '',
            secureToggle ? 'pr-12' : '',
            className,
          )}
          placeholderTextColor="#71717a"
          secureTextEntry={secure}
          {...props}
        />
        {secureToggle ? (
          <Pressable
            className="absolute right-3 top-3"
            onPress={() => setSecure((v) => !v)}
            accessibilityLabel="Toggle password visibility"
          >
            <Text variant="muted">{secure ? 'Show' : 'Hide'}</Text>
          </Pressable>
        ) : null}
      </View>
      {error ? <Text variant="error">{error}</Text> : null}
    </View>
  );
}
