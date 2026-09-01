import { ActivityIndicator, View } from 'react-native';
import { NavigationContainer } from '@react-navigation/native';
import { createNativeStackNavigator } from '@react-navigation/native-stack';
import { useAuth } from '@/contexts/AuthContext';
import { LoginScreen } from '@/screens/LoginScreen';
import { ProductsScreen } from '@/screens/ProductsScreen';
import { ProductDetailScreen } from '@/screens/ProductDetailScreen';
import { CategoriesScreen } from '@/screens/CategoriesScreen';
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

function AppNavigator() {
  return (
    <AppStack.Navigator>
      <AppStack.Screen name="ProductsList" component={ProductsScreen} options={{ title: 'Products' }} />
      <AppStack.Screen name="ProductDetail" component={ProductDetailScreen} options={{ title: 'Product' }} />
      <AppStack.Screen name="CategoriesList" component={CategoriesScreen} options={{ title: 'Categories' }} />
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
