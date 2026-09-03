import { ActivityIndicator, View } from 'react-native';
import { NavigationContainer } from '@react-navigation/native';
import { createNativeStackNavigator } from '@react-navigation/native-stack';
import { useAuth } from '@/contexts/AuthContext';
import { LoginScreen } from '@/screens/LoginScreen';
import { ProductsScreen } from '@/screens/ProductsScreen';
import { ProductDetailScreen } from '@/screens/ProductDetailScreen';
import { CategoriesScreen } from '@/screens/CategoriesScreen';
import { PermissionGate } from '@/components/PermissionGate';
import { Permissions } from '@/api/types';
import type { AppStackParamList, AuthStackParamList } from '@/navigation/types';

const AuthStack = createNativeStackNavigator<AuthStackParamList>();
const AppStack = createNativeStackNavigator<AppStackParamList>();

function AuthNavigator() {
  return (
    <AuthStack.Navigator screenOptions={{ headerShown: false }}>
      <AuthStack.Screen name="Login" component={LoginScreen} />
    </AuthStack.Navigator>
  );
}

function GatedProductsScreen() {
  return (
    <PermissionGate permission={Permissions.Catalog.Products.Read}>
      <ProductsScreen />
    </PermissionGate>
  );
}

function GatedProductDetailScreen() {
  return (
    <PermissionGate permission={Permissions.Catalog.Products.Read}>
      <ProductDetailScreen />
    </PermissionGate>
  );
}

function GatedCategoriesScreen() {
  return (
    <PermissionGate permission={Permissions.Catalog.Categories.Read}>
      <CategoriesScreen />
    </PermissionGate>
  );
}

function AppNavigator() {
  return (
    <AppStack.Navigator>
      <AppStack.Screen name="ProductsList" component={GatedProductsScreen} options={{ title: 'Products' }} />
      <AppStack.Screen name="ProductDetail" component={GatedProductDetailScreen} options={{ title: 'Product' }} />
      <AppStack.Screen name="CategoriesList" component={GatedCategoriesScreen} options={{ title: 'Categories' }} />
    </AppStack.Navigator>
  );
}

function BootSplash() {
  return (
    <View className="flex-1 items-center justify-center bg-background">
      <ActivityIndicator size="large" color="#5b5bd6" />
    </View>
  );
}

export function RootNavigator() {
  const { status } = useAuth();

  return (
    <NavigationContainer>
      {status === 'bootstrapping' ? (
        <BootSplash />
      ) : status === 'authenticated' ? (
        <AppNavigator />
      ) : (
        <AuthNavigator />
      )}
    </NavigationContainer>
  );
}
