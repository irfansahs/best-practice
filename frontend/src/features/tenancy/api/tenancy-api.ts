import { baseApi } from '@/shared/api/base-api';
import type {
  ApiResponse,
  MemberListItem,
  OrganizationListItem,
  PermissionCatalogItem,
  RoleListItem,
} from '@/shared/api/api-types';

export const tenancyApi = baseApi.injectEndpoints({
  endpoints: (builder) => ({
    getOrganizations: builder.query<ApiResponse<OrganizationListItem[]>, void>({
      query: () => ({ url: '/tenancy/organizations' }),
      providesTags: [{ type: 'Organizations', id: 'LIST' }],
    }),
    createOrganization: builder.mutation<
      ApiResponse<{ id: string; slug: string }>,
      { name: string; slug: string; parentId?: string | null; contactEmail?: string; timeZoneId?: string; defaultCulture?: string }
    >({
      query: (body) => ({ url: '/tenancy/organizations', method: 'POST', data: body }),
      invalidatesTags: [{ type: 'Organizations', id: 'LIST' }],
    }),
    updateOrganization: builder.mutation<
      void,
      { id: string; name: string; contactEmail?: string | null; timeZoneId?: string; defaultCulture?: string }
    >({
      query: ({ id, ...body }) => ({ url: `/tenancy/organizations/${id}`, method: 'PUT', data: body }),
      invalidatesTags: [{ type: 'Organizations', id: 'LIST' }],
    }),
    changeOrganizationStatus: builder.mutation<void, { id: string; status: string }>({
      query: ({ id, status }) => ({
        url: `/tenancy/organizations/${id}/status`,
        method: 'POST',
        data: { status },
      }),
      invalidatesTags: [{ type: 'Organizations', id: 'LIST' }],
    }),
    getMembers: builder.query<ApiResponse<MemberListItem[]>, string>({
      query: (organizationId) => ({ url: `/tenancy/organizations/${organizationId}/members` }),
      providesTags: (_result, _error, organizationId) => [{ type: 'Members', id: organizationId }],
    }),
    addMember: builder.mutation<
      ApiResponse<{ membershipId: string }>,
      { organizationId: string; userId: string; roleIds: string[]; title?: string; isPrimary?: boolean }
    >({
      query: ({ organizationId, ...body }) => ({
        url: `/tenancy/organizations/${organizationId}/members`,
        method: 'POST',
        data: body,
      }),
      invalidatesTags: (_result, _error, { organizationId }) => [{ type: 'Members', id: organizationId }],
    }),
    updateMemberRoles: builder.mutation<void, { membershipId: string; organizationId: string; roleIds: string[] }>({
      query: ({ membershipId, roleIds }) => ({
        url: `/tenancy/members/${membershipId}/roles`,
        method: 'PUT',
        data: { roleIds },
      }),
      invalidatesTags: (_result, _error, { organizationId }) => [{ type: 'Members', id: organizationId }],
    }),
    changeMemberStatus: builder.mutation<void, { membershipId: string; organizationId: string; status: string }>({
      query: ({ membershipId, status }) => ({
        url: `/tenancy/members/${membershipId}/status`,
        method: 'POST',
        data: { status },
      }),
      invalidatesTags: (_result, _error, { organizationId }) => [{ type: 'Members', id: organizationId }],
    }),
    getRoles: builder.query<ApiResponse<RoleListItem[]>, void>({
      query: () => ({ url: '/tenancy/roles' }),
      providesTags: [{ type: 'Roles', id: 'LIST' }],
    }),
    createRole: builder.mutation<
      ApiResponse<{ id: string }>,
      { name: string; description?: string; allowedClients: number; grants: { permissionId: string; scope: number }[] }
    >({
      query: (body) => ({ url: '/tenancy/roles', method: 'POST', data: body }),
      invalidatesTags: [{ type: 'Roles', id: 'LIST' }],
    }),
    updateRolePermissions: builder.mutation<
      void,
      { roleId: string; grants: { permissionId: string; scope: number }[] }
    >({
      query: ({ roleId, grants }) => ({
        url: `/tenancy/roles/${roleId}/permissions`,
        method: 'PUT',
        data: { grants },
      }),
      invalidatesTags: [{ type: 'Roles', id: 'LIST' }],
    }),
    getPermissionCatalog: builder.query<ApiResponse<PermissionCatalogItem[]>, void>({
      query: () => ({ url: '/tenancy/permissions' }),
      providesTags: ['PermissionCatalog'],
    }),
  }),
});

export const {
  useGetOrganizationsQuery,
  useCreateOrganizationMutation,
  useUpdateOrganizationMutation,
  useChangeOrganizationStatusMutation,
  useGetMembersQuery,
  useAddMemberMutation,
  useUpdateMemberRolesMutation,
  useChangeMemberStatusMutation,
  useGetRolesQuery,
  useCreateRoleMutation,
  useUpdateRolePermissionsMutation,
  useGetPermissionCatalogQuery,
} = tenancyApi;
