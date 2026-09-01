import { apiClient } from '@/api/client';
import type { ApiResponse, CategoryListItem } from '@/api/types';

export async function getCategories() {
  const { data } = await apiClient.get<ApiResponse<CategoryListItem[]>>('/catalog/categories');
  return data.data;
}
