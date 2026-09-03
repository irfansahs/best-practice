import { baseApi } from '@/shared/api/base-api';
import type { ApiResponse, CurrentUser, LoginResponse, RefreshTokenResponse } from '@/shared/api/api-types';
import { getRefreshToken } from '@/features/auth/token-storage';

export const authApi = baseApi.injectEndpoints({
  endpoints: (builder) => ({
    login: builder.mutation<ApiResponse<LoginResponse>, { email: string; password: string }>({
      query: (body) => ({
        url: '/auth/login',
        method: 'POST',
        data: { ...body, clientType: 'web' },
      }),
    }),
    refresh: builder.mutation<ApiResponse<RefreshTokenResponse>, void>({
      query: () => {
        const refreshToken = getRefreshToken();
        return {
          url: '/auth/refresh',
          method: 'POST',
          data: { refreshToken },
        };
      },
    }),
    logout: builder.mutation<void, void>({
      query: () => {
        const refreshToken = getRefreshToken();
        return {
          url: '/auth/logout',
          method: 'POST',
          data: { refreshToken },
        };
      },
    }),
    switchOrganization: builder.mutation<ApiResponse<LoginResponse>, { organizationId: string }>({
      query: ({ organizationId }) => ({
        url: '/auth/switch-organization',
        method: 'POST',
        data: {
          organizationId,
          refreshToken: getRefreshToken(),
          clientType: 'web',
        },
      }),
    }),
    getCurrentUser: builder.query<ApiResponse<CurrentUser>, void>({
      query: () => ({
        url: '/auth/me',
      }),
      providesTags: ['CurrentUser'],
    }),
  }),
});

export const {
  useLoginMutation,
  useRefreshMutation,
  useLogoutMutation,
  useSwitchOrganizationMutation,
  useGetCurrentUserQuery,
} = authApi;
