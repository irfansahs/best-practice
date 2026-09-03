import { createAsyncThunk, createSlice, type PayloadAction } from '@reduxjs/toolkit';
import type { RootState } from '@/app/store';
import { PermissionScope, type CurrentUser } from '@/shared/api/api-types';
import { setAccessToken, getAccessToken } from '@/shared/api/axios-base-query';
import { authApi } from '@/features/auth/api/auth-api';
import { baseApi } from '@/shared/api/base-api';
import { clearTokens, getRefreshToken, setRefreshToken } from '@/features/auth/token-storage';

export type AuthStatus = 'idle' | 'bootstrapping' | 'authenticated' | 'unauthenticated';

interface AuthState {
  status: AuthStatus;
  user: CurrentUser | null;
  expiresAt: string | null;
  error: string | null;
}

const initialState: AuthState = {
  status: 'idle',
  user: null,
  expiresAt: null,
  error: null,
};

function permissionMap(user: CurrentUser | null): Record<string, number> {
  return user?.permissions ?? {};
}

export const bootstrapAuth = createAsyncThunk('auth/bootstrap', async (_, { dispatch, rejectWithValue }) => {
  const existingToken = getAccessToken();

  if (!existingToken) {
    const storedRefresh = getRefreshToken();
    if (!storedRefresh) {
      return rejectWithValue('unauthenticated');
    }

    const refreshResult = await dispatch(authApi.endpoints.refresh.initiate()).unwrap();
    setAccessToken(refreshResult.data.accessToken);
    setRefreshToken(refreshResult.data.refreshToken);
  }

  const meResult = await dispatch(authApi.endpoints.getCurrentUser.initiate()).unwrap();
  return {
    user: meResult.data,
    expiresAt: null as string | null,
  };
});

export const login = createAsyncThunk(
  'auth/login',
  async (credentials: { email: string; password: string }, { dispatch, rejectWithValue }) => {
    try {
      const result = await dispatch(authApi.endpoints.login.initiate(credentials)).unwrap();
      setAccessToken(result.data.accessToken);
      setRefreshToken(result.data.refreshToken);
      const meResult = await dispatch(authApi.endpoints.getCurrentUser.initiate()).unwrap();
      return {
        user: meResult.data,
        expiresAt: result.data.expiresAt,
      };
    } catch {
      return rejectWithValue('Auth.Login.Error');
    }
  },
);

export const switchOrganization = createAsyncThunk(
  'auth/switchOrganization',
  async (organizationId: string, { dispatch, rejectWithValue }) => {
    try {
      const result = await dispatch(authApi.endpoints.switchOrganization.initiate({ organizationId })).unwrap();
      setAccessToken(result.data.accessToken);
      setRefreshToken(result.data.refreshToken);
      dispatch(baseApi.util.resetApiState());
      const meResult = await dispatch(authApi.endpoints.getCurrentUser.initiate()).unwrap();
      return {
        user: meResult.data,
        expiresAt: result.data.expiresAt,
      };
    } catch {
      return rejectWithValue('Auth.Switch.Error');
    }
  },
);

export const logout = createAsyncThunk('auth/logout', async (_, { dispatch }) => {
  try {
    await dispatch(authApi.endpoints.logout.initiate()).unwrap();
  } catch {
    // ignore logout errors
  }
  setAccessToken(null);
  clearTokens();
  dispatch(baseApi.util.resetApiState());
});

export const forceSessionExpired = createAsyncThunk('auth/sessionExpired', async (_, { dispatch }) => {
  setAccessToken(null);
  clearTokens();
  dispatch(baseApi.util.resetApiState());
});

const authSlice = createSlice({
  name: 'auth',
  initialState,
  reducers: {
    clearAuthError(state) {
      state.error = null;
    },
    setUser(state, action: PayloadAction<CurrentUser | null>) {
      state.user = action.payload;
      state.status = action.payload ? 'authenticated' : 'unauthenticated';
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(bootstrapAuth.pending, (state) => {
        state.status = 'bootstrapping';
        state.error = null;
      })
      .addCase(bootstrapAuth.fulfilled, (state, action) => {
        state.status = 'authenticated';
        state.user = action.payload.user;
        state.expiresAt = action.payload.expiresAt;
      })
      .addCase(bootstrapAuth.rejected, (state) => {
        state.status = 'unauthenticated';
        state.user = null;
        setAccessToken(null);
        clearTokens();
      })
      .addCase(login.fulfilled, (state, action) => {
        state.status = 'authenticated';
        state.user = action.payload.user;
        state.expiresAt = action.payload.expiresAt;
        state.error = null;
      })
      .addCase(login.rejected, (state, action) => {
        state.status = 'unauthenticated';
        state.error = (action.payload as string | undefined) ?? 'Auth.Login.Error';
      })
      .addCase(switchOrganization.fulfilled, (state, action) => {
        state.user = action.payload.user;
        state.expiresAt = action.payload.expiresAt;
        state.error = null;
      })
      .addCase(logout.fulfilled, (state) => {
        state.status = 'unauthenticated';
        state.user = null;
        state.expiresAt = null;
      })
      .addCase(forceSessionExpired.fulfilled, (state) => {
        state.status = 'unauthenticated';
        state.user = null;
        state.expiresAt = null;
      });
  },
});

export const { clearAuthError, setUser } = authSlice.actions;
export default authSlice.reducer;

export const selectAuthStatus = (state: RootState) => state.auth.status;
export const selectCurrentUser = (state: RootState) => state.auth.user;
export const selectAuthError = (state: RootState) => state.auth.error;
export const selectIsAuthenticated = (state: RootState) => state.auth.status === 'authenticated';
export const selectActiveOrganization = (state: RootState) => state.auth.user?.activeOrganization ?? null;
export const selectOrganizations = (state: RootState) => state.auth.user?.organizations ?? [];

export const selectPermissions = (state: RootState): Record<string, number> =>
  permissionMap(state.auth.user);

/** Prefer `usePermission` — this factory recreates a selector per call. */
export const selectHasPermission =
  (permission: string, minScope: number = PermissionScope.Organization) =>
  (state: RootState) =>
    (selectPermissions(state)[permission] ?? -1) >= minScope;

export const selectHasAnyPermission =
  (...permissions: string[]) =>
  (state: RootState) => {
    const map = selectPermissions(state);
    return permissions.some((p) => (map[p] ?? -1) >= PermissionScope.Organization);
  };
