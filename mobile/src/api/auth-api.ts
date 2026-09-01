import { apiClient } from '@/api/client';
import type { ApiResponse, CurrentUserDto, LoginResponse } from '@/api/types';
import { clearTokens, setAccessToken, setRefreshToken, getRefreshToken } from '@/services/token-store';

export async function login(email: string, password: string) {
  const { data } = await apiClient.post<ApiResponse<LoginResponse>>('/auth/login', { email, password });
  setAccessToken(data.data.accessToken);
  await setRefreshToken(data.data.refreshToken);
  return data.data;
}

export async function logout() {
  const refresh = await getRefreshToken();
  if (refresh) {
    try {
      await apiClient.post('/auth/logout', { refreshToken: refresh });
    } catch {
      // ignore logout errors
    }
  }
  await clearTokens();
}

export async function getCurrentUser() {
  const { data } = await apiClient.get<ApiResponse<CurrentUserDto>>('/auth/me');
  return data.data;
}
