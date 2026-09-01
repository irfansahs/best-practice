import { useState } from 'react';
import {
  KeyboardAvoidingView,
  Platform,
  ScrollView,
  View,
} from 'react-native';
import { SafeAreaView } from 'react-native-safe-area-context';
import { getApiErrorMessage } from '@/api/client';
import { isProblemDetails, getValidationErrors } from '@/api/problem-details';
import axios from 'axios';
import { useAuth } from '@/contexts/AuthContext';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Input } from '@/components/ui/input';
import { Text } from '@/components/ui/text';

export function LoginScreen() {
  const { login } = useAuth();
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [fieldErrors, setFieldErrors] = useState<Record<string, string[]>>({});

  const handleLogin = async () => {
    setError(null);
    setFieldErrors({});

    const trimmedEmail = email.trim();
    if (!trimmedEmail) {
      setFieldErrors({ email: ['Email is required'] });
      return;
    }
    if (!password) {
      setFieldErrors({ password: ['Password is required'] });
      return;
    }

    setLoading(true);
    try {
      await login(trimmedEmail, password);
    } catch (e) {
      if (axios.isAxiosError(e) && isProblemDetails(e.response?.data)) {
        const validation = getValidationErrors(e.response.data);
        if (validation) setFieldErrors(validation);
        setError(getApiErrorMessage(e));
      } else {
        setError(getApiErrorMessage(e, 'Login failed'));
      }
    } finally {
      setLoading(false);
    }
  };

  return (
    <SafeAreaView className="flex-1 bg-background">
      <KeyboardAvoidingView
        className="flex-1"
        behavior={Platform.OS === 'ios' ? 'padding' : undefined}
      >
        <ScrollView
          contentContainerClassName="flex-grow justify-center p-4"
          keyboardShouldPersistTaps="handled"
        >
          <Card className="mx-auto w-full max-w-md">
            <CardHeader>
              <CardTitle>Sign in</CardTitle>
              <CardDescription>Enter your credentials to access the catalog.</CardDescription>
            </CardHeader>
            <CardContent className="gap-4">
              <Input
                label="Email"
                value={email}
                onChangeText={setEmail}
                autoCapitalize="none"
                keyboardType="email-address"
                placeholder="admin@local.dev"
                error={fieldErrors.email?.[0]}
              />
              <Input
                label="Password"
                value={password}
                onChangeText={setPassword}
                secureToggle
                placeholder="Password"
                error={fieldErrors.password?.[0]}
              />
              {error ? <Text variant="error">{error}</Text> : null}
              <Button loading={loading} onPress={() => void handleLogin()}>
                Sign in
              </Button>
            </CardContent>
          </Card>
        </ScrollView>
      </KeyboardAvoidingView>
    </SafeAreaView>
  );
}
