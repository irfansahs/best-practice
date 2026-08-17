import { createSlice, type PayloadAction } from '@reduxjs/toolkit';
import type { RootState } from '@/app/store';

interface ProductsUiState {
  page: number;
  pageSize: number;
  search: string;
}

const initialState: ProductsUiState = {
  page: 1,
  pageSize: 20,
  search: '',
};

const productsSlice = createSlice({
  name: 'productsUi',
  initialState,
  reducers: {
    setPage(state, action: PayloadAction<number>) {
      state.page = action.payload;
    },
    setPageSize(state, action: PayloadAction<number>) {
      state.pageSize = action.payload;
      state.page = 1;
    },
    setSearch(state, action: PayloadAction<string>) {
      state.search = action.payload;
      state.page = 1;
    },
  },
});

export const { setPage, setPageSize, setSearch } = productsSlice.actions;
export default productsSlice.reducer;

export const selectProductsPage = (state: RootState) => state.productsUi.page;
export const selectProductsPageSize = (state: RootState) => state.productsUi.pageSize;
export const selectProductsSearch = (state: RootState) => state.productsUi.search;
