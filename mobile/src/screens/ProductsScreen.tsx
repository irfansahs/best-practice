import { useCallback, useEffect, useRef, useState } from 'react';
import {
  ActivityIndicator,
  FlatList,
  Pressable,
  RefreshControl,
  View,
} from 'react-native';
import { useNavigation } from '@react-navigation/native';
import type { NativeStackNavigationProp } from '@react-navigation/native-stack';
import { getApiErrorMessage } from '@/api/client';
import { getProducts } from '@/api/products-api';
import type { ProductListItem } from '@/api/types';
import type { AppStackParamList } from '@/navigation/types';
import { useAuth } from '@/contexts/AuthContext';
import { Can } from '@/components/Can';
import { Permissions } from '@/api/types';
import { Button } from '@/components/ui/button';
import { Card, CardContent } from '@/components/ui/card';
import { Input } from '@/components/ui/input';
import { StatusBadge } from '@/components/ui/badge';
import { Text } from '@/components/ui/text';

const PAGE_SIZE = 20;

export function ProductsScreen() {
  const navigation = useNavigation<NativeStackNavigationProp<AppStackParamList, 'ProductsList'>>();
  const { logout, activeOrganization, switchOrganization, user } = useAuth();
  const [items, setItems] = useState<ProductListItem[]>([]);
  const [page, setPage] = useState(1);
  const [hasNext, setHasNext] = useState(false);
  const [search, setSearch] = useState('');
  const [debouncedSearch, setDebouncedSearch] = useState('');
  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);
  const [loadingMore, setLoadingMore] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const loadingMoreRef = useRef(false);

  useEffect(() => {
    const timer = setTimeout(() => setDebouncedSearch(search.trim()), 300);
    return () => clearTimeout(timer);
  }, [search]);

  const loadPage = useCallback(
    async (pageToLoad: number, searchTerm: string, append: boolean) => {
      if (append) {
        setLoadingMore(true);
        loadingMoreRef.current = true;
      } else {
        setLoading(true);
      }
      setError(null);

      try {
        const result = await getProducts({ page: pageToLoad, pageSize: PAGE_SIZE, search: searchTerm });
        setItems((prev) => (append ? [...prev, ...result.items] : result.items));
        setPage(result.page);
        setHasNext(result.hasNext);
      } catch (e) {
        setError(getApiErrorMessage(e));
        if (!append) setItems([]);
      } finally {
        setLoading(false);
        setRefreshing(false);
        setLoadingMore(false);
        loadingMoreRef.current = false;
      }
    },
    [],
  );

  useEffect(() => {
    void loadPage(1, debouncedSearch, false);
  }, [debouncedSearch, loadPage]);

  const onRefresh = () => {
    setRefreshing(true);
    void loadPage(1, debouncedSearch, false);
  };

  const onEndReached = () => {
    if (!hasNext || loadingMoreRef.current || loading) return;
    void loadPage(page + 1, debouncedSearch, true);
  };

  return (
    <View className="flex-1 bg-background">
      <View className="flex-row items-center justify-between px-4 py-3 border-b border-border">
        <View>
          <Text variant="h2">Products</Text>
          {activeOrganization ? (
            <Text variant="muted">{activeOrganization.name}</Text>
          ) : null}
        </View>
        <View className="flex-row gap-2">
          <Can permission={Permissions.Catalog.Categories.Read}>
            <Button variant="ghost" size="sm" onPress={() => navigation.navigate('CategoriesList')}>
              Categories
            </Button>
          </Can>
          {user?.organizations?.length ? (
            <Button
              variant="ghost"
              size="sm"
              onPress={() => {
                const next = user.organizations.find((org) => org.id !== activeOrganization?.id);
                if (next) void switchOrganization(next.id);
              }}
            >
              Switch
            </Button>
          ) : null}
          <Button variant="ghost" size="sm" onPress={() => void logout()}>
            Logout
          </Button>
        </View>
      </View>

      <View className="px-4 py-3">
        <Input
          placeholder="Search products..."
          value={search}
          onChangeText={setSearch}
          autoCapitalize="none"
        />
      </View>

      {loading && items.length === 0 ? (
        <View className="flex-1 items-center justify-center">
          <ActivityIndicator size="large" color="#5b5bd6" />
        </View>
      ) : null}

      {error ? (
        <View className="px-4">
          <Text variant="error">{error}</Text>
        </View>
      ) : null}

      <FlatList
        data={items}
        keyExtractor={(item) => item.id}
        contentContainerClassName="px-4 pb-4 gap-3"
        refreshControl={<RefreshControl refreshing={refreshing} onRefresh={onRefresh} />}
        onEndReached={onEndReached}
        onEndReachedThreshold={0.4}
        ListEmptyComponent={
          !loading ? (
            <Text variant="muted" className="text-center py-8">No products found</Text>
          ) : null
        }
        ListFooterComponent={
          loadingMore ? (
            <View className="py-4">
              <ActivityIndicator color="#5b5bd6" />
            </View>
          ) : null
        }
        renderItem={({ item }) => (
          <Pressable onPress={() => navigation.navigate('ProductDetail', { id: item.id })}>
            <Card>
              <CardContent className="gap-2 pt-4">
                <View className="flex-row items-center justify-between">
                  <Text className="font-mono font-semibold">{item.sku}</Text>
                  <StatusBadge active={item.isActive} />
                </View>
                <Text variant="h3">{item.name}</Text>
                <Text variant="muted">
                  {item.price.toFixed(2)} {item.currency}
                </Text>
              </CardContent>
            </Card>
          </Pressable>
        )}
      />
    </View>
  );
}
