import * as React from 'react';
import { bootstrapSession, setSessionExpiredHandler } from '@/api/client';
import { getCurrentUser, login as apiLogin, logout as apiLogout } from '@/api/auth-api';
import { loadCulture } from '@/services/culture-store';
import type { CurrentUserDto } from '@/api/types';

export type AuthStatus = 'bootstrapping' | 'authenticated' | 'unauthenticated';

interface AuthContextValue {
  status: AuthStatus;
  user: CurrentUserDto | null;
  login: (email: string, password: string) => Promise<void>;
  logout: () => Promise<void>;
}

const AuthContext = React.createContext<AuthContextValue | null>(null);

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [status, setStatus] = React.useState<AuthStatus>('bootstrapping');
  const [user, setUser] = React.useState<CurrentUserDto | null>(null);

  const logout = React.useCallback(async () => {
    await apiLogout();
    setUser(null);
    setStatus('unauthenticated');
  }, []);

  const bootstrapAuth = React.useCallback(async () => {
    await loadCulture();
    const ok = await bootstrapSession();
    if (!ok) {
      setUser(null);
      setStatus('unauthenticated');
      return;
    }
    try {
      const me = await getCurrentUser();
      setUser(me);
      setStatus('authenticated');
    } catch {
      await apiLogout();
      setUser(null);
      setStatus('unauthenticated');
    }
  }, []);

  React.useEffect(() => {
    setSessionExpiredHandler(() => {
      void logout();
    });
    void bootstrapAuth();
    return () => setSessionExpiredHandler(null);
  }, [bootstrapAuth, logout]);

  const login = React.useCallback(async (email: string, password: string) => {
    await apiLogin(email, password);
    const me = await getCurrentUser();
    setUser(me);
    setStatus('authenticated');
  }, []);

  return (
    <AuthContext.Provider value={{ status, user, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const ctx = React.useContext(AuthContext);
  if (!ctx) throw new Error('useAuth must be used within AuthProvider');
  return ctx;
}
