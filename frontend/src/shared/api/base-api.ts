import { createApi } from '@reduxjs/toolkit/query/react';
import { axiosBaseQuery } from './axios-base-query';

export const baseApi = createApi({
  reducerPath: 'api',
  baseQuery: axiosBaseQuery,
  tagTypes: ['Products', 'Product', 'Categories', 'Category', 'Languages', 'CurrentUser', 'Resources'],
  endpoints: () => ({}),
});
