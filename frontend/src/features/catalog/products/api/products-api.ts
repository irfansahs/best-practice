import { baseApi } from '@/shared/api/base-api';
import type {
  ApiResponse,
  CreateProductResponse,
  PagedList,
  ProductDetail,
  ProductListItem,
} from '@/shared/api/api-types';

export const productsApi = baseApi.injectEndpoints({
  endpoints: (builder) => ({
    getProducts: builder.query<
      ApiResponse<PagedList<ProductListItem>>,
      { page: number; pageSize: number; search?: string }
    >({
      query: ({ page, pageSize, search }) => ({
        url: '/catalog/products',
        params: { page, pageSize, search: search || undefined },
      }),
      providesTags: (result) =>
        result
          ? [
              ...result.data.items.map(({ id }) => ({ type: 'Product' as const, id })),
              { type: 'Products', id: 'LIST' },
            ]
          : [{ type: 'Products', id: 'LIST' }],
    }),
    getProduct: builder.query<ApiResponse<ProductDetail>, string>({
      query: (id) => ({
        url: `/catalog/products/${id}`,
      }),
      providesTags: (_result, _error, id) => [{ type: 'Product', id }],
    }),
    createProduct: builder.mutation<
      ApiResponse<CreateProductResponse>,
      {
        sku: string;
        price: number;
        currency: string;
        categoryId: string;
        languageId: string;
        name: string;
        description?: string;
      }
    >({
      query: (body) => ({
        url: '/catalog/products',
        method: 'POST',
        data: body,
      }),
      invalidatesTags: [{ type: 'Products', id: 'LIST' }],
    }),
    updateProduct: builder.mutation<
      void,
      {
        id: string;
        categoryId: string;
        languageId: string;
        name: string;
        description?: string;
        isActive: boolean;
      }
    >({
      query: ({ id, ...body }) => ({
        url: `/catalog/products/${id}`,
        method: 'PUT',
        data: body,
      }),
      invalidatesTags: (_result, _error, { id }) => [
        { type: 'Product', id },
        { type: 'Products', id: 'LIST' },
      ],
    }),
    changeProductPrice: builder.mutation<void, { id: string; price: number; currency: string }>({
      query: ({ id, price, currency }) => ({
        url: `/catalog/products/${id}/price`,
        method: 'PUT',
        data: { price, currency },
      }),
      invalidatesTags: (_result, _error, { id }) => [
        { type: 'Product', id },
        { type: 'Products', id: 'LIST' },
      ],
    }),
    deleteProduct: builder.mutation<void, string>({
      query: (id) => ({
        url: `/catalog/products/${id}`,
        method: 'DELETE',
      }),
      invalidatesTags: [{ type: 'Products', id: 'LIST' }],
    }),
  }),
});

export const {
  useGetProductsQuery,
  useGetProductQuery,
  useCreateProductMutation,
  useUpdateProductMutation,
  useChangeProductPriceMutation,
  useDeleteProductMutation,
} = productsApi;
