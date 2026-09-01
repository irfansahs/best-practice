import axios, { type AxiosError, type InternalAxiosRequestConfig } from 'axios';
import { API_BASE_URL } from '../config';
import {
  clearTokens,
  getAccessToken,
  getRefreshToken,
  setAccessToken,
  setRefreshToken,
} from '../auth/token-storage';
import type { ApiResponse, LoginResponse, PagedList, ProductListItem } from './types';

export const apiClient = axios.create({
  baseURL: API_BASE_URL,
  timeout: 15000,
});

let refreshPromise: Promise<string | null> | null = null;
let onSessionExpired: (() => void) | null = null;

export function setSessionExpiredHandler(handler: (() => void) | null) {
  onSessionExpired = handler;
}

apiClient.interceptors.request.use((config) => {
  const token = getAccessToken();
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});

async function refreshAccessToken(): Promise<string | null> {
  if (!refreshPromise) {
    const storedRefresh = await getRefreshToken();
    if (!storedRefresh) {
      setAccessToken(null);
      await clearTokens();
      onSessionExpired?.();
      return null;
    }

    refreshPromise = apiClient
      .post<ApiResponse<LoginResponse>>('/auth/refresh', { refreshToken: storedRefresh })
      .then(async (response) => {
        const data = response.data.data;
        setAccessToken(data.accessToken);
        await setRefreshToken(data.refreshToken);
        return data.accessToken;
      })
      .catch(async () => {
        setAccessToken(null);
        await clearTokens();
        onSessionExpired?.();
        return null;
      })
      .finally(() => {
        refreshPromise = null;
      });
  }

  return refreshPromise;
}

apiClient.interceptors.response.use(
  (response) => response,
  async (error: AxiosError) => {
    const originalRequest = error.config as (InternalAxiosRequestConfig & { _retry?: boolean }) | undefined;

    if (error.response?.status === 401 && originalRequest && !originalRequest._retry) {
      const isAuthEndpoint =
        originalRequest.url?.includes('/auth/login') ||
        originalRequest.url?.includes('/auth/refresh') ||
        originalRequest.url?.includes('/auth/register') ||
        originalRequest.url?.includes('/auth/logout');

      if (!isAuthEndpoint) {
        originalRequest._retry = true;
        const newToken = await refreshAccessToken();
        if (newToken) {
          originalRequest.headers.Authorization = `Bearer ${newToken}`;
          return apiClient(originalRequest);
        }
      }
    }

    return Promise.reject(error);
  },
);

export async function bootstrapSession() {
  const refresh = await getRefreshToken();
  if (!refresh) return false;

  const token = await refreshAccessToken();
  return token !== null;
}

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

export async function getProducts(page = 1, pageSize = 20) {
  const { data } = await apiClient.get<ApiResponse<PagedList<ProductListItem>>>('/catalog/products', {
    params: { page, pageSize },
  });
  return data.data;
}
