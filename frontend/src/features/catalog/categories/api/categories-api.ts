import { baseApi } from '@/shared/api/base-api';
import type { ApiResponse, CategoryDetail, CategoryListItem, CreateCategoryResponse } from '@/shared/api/api-types';

export const categoriesApi = baseApi.injectEndpoints({
  endpoints: (builder) => ({
    getCategories: builder.query<ApiResponse<CategoryListItem[]>, void>({
      query: () => ({ url: '/catalog/categories' }),
      providesTags: [{ type: 'Categories', id: 'LIST' }],
    }),
    getCategory: builder.query<ApiResponse<CategoryDetail>, string>({
      query: (id) => ({ url: `/catalog/categories/${id}` }),
      providesTags: (_result, _error, id) => [{ type: 'Category', id }],
    }),
    createCategory: builder.mutation<
      ApiResponse<CreateCategoryResponse>,
      { parentCategoryId?: string | null; languageId: string; name: string; description?: string }
    >({
      query: (body) => ({
        url: '/catalog/categories',
        method: 'POST',
        data: body,
      }),
      invalidatesTags: [{ type: 'Categories', id: 'LIST' }],
    }),
    updateCategory: builder.mutation<
      void,
      {
        id: string;
        parentCategoryId?: string | null;
        languageId: string;
        name: string;
        description?: string;
        isActive: boolean;
      }
    >({
      query: ({ id, ...body }) => ({
        url: `/catalog/categories/${id}`,
        method: 'PUT',
        data: body,
      }),
      invalidatesTags: (_result, _error, { id }) => [
        { type: 'Category', id },
        { type: 'Categories', id: 'LIST' },
      ],
    }),
    deleteCategory: builder.mutation<void, string>({
      query: (id) => ({
        url: `/catalog/categories/${id}`,
        method: 'DELETE',
      }),
      invalidatesTags: [{ type: 'Categories', id: 'LIST' }],
    }),
  }),
});

export const {
  useGetCategoriesQuery,
  useGetCategoryQuery,
  useCreateCategoryMutation,
  useUpdateCategoryMutation,
  useDeleteCategoryMutation,
} = categoriesApi;
