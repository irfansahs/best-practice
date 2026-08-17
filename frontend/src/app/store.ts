import { configureStore } from '@reduxjs/toolkit';
import { baseApi } from '@/shared/api/base-api';
import authReducer from '@/features/auth/slice/auth-slice';
import productsUiReducer from '@/features/catalog/products/slice/products-slice';

export const store = configureStore({
  reducer: {
    auth: authReducer,
    productsUi: productsUiReducer,
    [baseApi.reducerPath]: baseApi.reducer,
  },
  middleware: (getDefaultMiddleware) => getDefaultMiddleware().concat(baseApi.middleware),
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
