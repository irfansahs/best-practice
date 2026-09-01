import { apiClient } from '@/api/client';
import type { ApiResponse, PagedList, ProductDetailDto, ProductListItem } from '@/api/types';

export interface GetProductsParams {
  page?: number;
  pageSize?: number;
  search?: string;
}

export async function getProducts(params: GetProductsParams = {}) {
  const { page = 1, pageSize = 20, search } = params;
  const { data } = await apiClient.get<ApiResponse<PagedList<ProductListItem>>>('/catalog/products', {
    params: { page, pageSize, search: search?.trim() || undefined },
  });
  return data.data;
}

export async function getProductById(id: string) {
  const { data } = await apiClient.get<ApiResponse<ProductDetailDto>>(`/catalog/products/${id}`);
  return data.data;
}
