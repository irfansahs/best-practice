export type AuthStackParamList = {
  Login: undefined;
};

export type AppStackParamList = {
  ProductsList: undefined;
  ProductDetail: { id: string };
  CategoriesList: undefined;
};

declare global {
  namespace ReactNavigation {
    interface RootParamList extends AppStackParamList {}
  }
}
