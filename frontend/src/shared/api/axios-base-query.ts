import axios, { type AxiosError, type AxiosRequestConfig } from 'axios';
import type { ProblemDetails } from './problem-details';
import { isProblemDetails, getProblemMessage } from './problem-details';
import {
  clearTokens,
  getRefreshToken,
  setRefreshToken,
} from '@/features/auth/token-storage';

export const API_BASE_URL = import.meta.env.VITE_API_URL;

let accessToken: string | null = null;
let refreshPromise: Promise<string | null> | null = null;
let onSessionExpired: (() => void) | null = null;

export function setAccessToken(token: string | null) {
  accessToken = token;
}

export function getAccessToken() {
  return accessToken;
}

export function setSessionExpiredHandler(handler: (() => void) | null) {
  onSessionExpired = handler;
}

export const axiosClient = axios.create({
  baseURL: API_BASE_URL,
  withCredentials: false,
  headers: {
    'Content-Type': 'application/json',
    Accept: 'application/json',
  },
});

axiosClient.interceptors.request.use((config) => {
  if (accessToken) {
    config.headers.Authorization = `Bearer ${accessToken}`;
  }
  config.headers['X-Culture'] = localStorage.getItem('culture') ?? 'en';
  return config;
});

async function refreshAccessToken(): Promise<string | null> {
  if (!refreshPromise) {
    const storedRefresh = getRefreshToken();
    if (!storedRefresh) {
      setAccessToken(null);
      clearTokens();
      onSessionExpired?.();
      return null;
    }

    refreshPromise = axiosClient
      .post('/auth/refresh', { refreshToken: storedRefresh })
      .then((response) => {
        const data = response.data?.data;
        const token = data?.accessToken as string | undefined;
        const nextRefresh = data?.refreshToken as string | undefined;
        setAccessToken(token ?? null);
        if (nextRefresh) {
          setRefreshToken(nextRefresh);
        }
        return token ?? null;
      })
      .catch(() => {
        setAccessToken(null);
        clearTokens();
        onSessionExpired?.();
        return null;
      })
      .finally(() => {
        refreshPromise = null;
      });
  }

  return refreshPromise;
}

axiosClient.interceptors.response.use(
  (response) => response,
  async (error: AxiosError) => {
    const originalRequest = error.config as (AxiosRequestConfig & { _retry?: boolean }) | undefined;

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
          originalRequest.headers = originalRequest.headers ?? {};
          originalRequest.headers.Authorization = `Bearer ${newToken}`;
          return axiosClient(originalRequest);
        }
      }
    }

    return Promise.reject(error);
  },
);

export interface AxiosBaseQueryArgs {
  url: string;
  method?: AxiosRequestConfig['method'];
  data?: unknown;
  params?: Record<string, unknown>;
  headers?: Record<string, string>;
}

export interface AxiosBaseQueryError {
  status?: number;
  data?: ProblemDetails | unknown;
  message: string;
}

export async function axiosBaseQuery<T>(
  args: AxiosBaseQueryArgs,
): Promise<{ data: T } | { error: AxiosBaseQueryError }> {
  try {
    const response = await axiosClient.request<T>({
      url: args.url,
      method: args.method ?? 'GET',
      data: args.data,
      params: args.params,
      headers: args.headers,
    });
    return { data: response.data };
  } catch (error) {
    if (axios.isAxiosError(error)) {
      const data = error.response?.data;
      const problem = isProblemDetails(data) ? data : undefined;
      return {
        error: {
          status: error.response?.status,
          data,
          message: problem ? getProblemMessage(problem) : error.message,
        },
      };
    }

    return {
      error: {
        message: error instanceof Error ? error.message : 'Unknown error',
      },
    };
  }
}
