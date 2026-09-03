import * as React from 'react';
import { bootstrapSession, setSessionExpiredHandler } from '@/api/client';
import { getCurrentUser, login as apiLogin, logout as apiLogout, switchOrganization as apiSwitch } from '@/api/auth-api';
import { loadCulture } from '@/services/culture-store';
import { PermissionScope, type CurrentUserDto, type OrganizationSummary } from '@/api/types';

export type AuthStatus = 'bootstrapping' | 'authenticated' | 'unauthenticated';

interface AuthContextValue {
  status: AuthStatus;
  user: CurrentUserDto | null;
  permissions: Record<string, number>;
  activeOrganization: OrganizationSummary | null;
  hasPermission: (permission: string, minScope?: number) => boolean;
  login: (email: string, password: string) => Promise<void>;
  logout: () => Promise<void>;
  switchOrganization: (organizationId: string) => Promise<void>;
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

  const switchOrganization = React.useCallback(async (organizationId: string) => {
    await apiSwitch(organizationId);
    const me = await getCurrentUser();
    setUser(me);
  }, []);

  const hasPermission = React.useCallback(
    (permission: string, minScope: number = PermissionScope.Organization) =>
      (user?.permissions[permission] ?? -1) >= minScope,
    [user],
  );

  const value = React.useMemo<AuthContextValue>(
    () => ({
      status,
      user,
      permissions: user?.permissions ?? {},
      activeOrganization: user?.activeOrganization ?? null,
      hasPermission,
      login,
      logout,
      switchOrganization,
    }),
    [status, user, hasPermission, login, logout, switchOrganization],
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const ctx = React.useContext(AuthContext);
  if (!ctx) throw new Error('useAuth must be used within AuthProvider');
  return ctx;
}
