import { useCallback, useEffect, useState } from 'react';
import { ActivityIndicator, FlatList, RefreshControl, View } from 'react-native';
import { getApiErrorMessage } from '@/api/client';
import { getCategories } from '@/api/categories-api';
import type { CategoryListItem } from '@/api/types';
import { Permissions } from '@/api/types';
import { Card, CardContent } from '@/components/ui/card';
import { StatusBadge } from '@/components/ui/badge';
import { Text } from '@/components/ui/text';
import { useAuth } from '@/contexts/AuthContext';

export function CategoriesScreen() {
  const { hasPermission } = useAuth();
  const canRead = hasPermission(Permissions.Catalog.Categories.Read);
  const [items, setItems] = useState<CategoryListItem[]>([]);
  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const load = useCallback(async (isRefresh = false) => {
    if (!canRead) return;
    if (isRefresh) setRefreshing(true);
    else setLoading(true);
    setError(null);
    try {
      const data = await getCategories();
      setItems(data);
    } catch (e) {
      setError(getApiErrorMessage(e));
      setItems([]);
    } finally {
      setLoading(false);
      setRefreshing(false);
    }
  }, [canRead]);

  useEffect(() => {
    void load();
  }, [load]);

  if (!canRead) {
    return (
      <View className="flex-1 items-center justify-center bg-background px-4">
        <Text variant="muted">You do not have permission to view categories.</Text>
      </View>
    );
  }

  if (loading && items.length === 0) {
    return (
      <View className="flex-1 items-center justify-center bg-background">
        <ActivityIndicator size="large" color="#5b5bd6" />
      </View>
    );
  }

  return (
    <View className="flex-1 bg-background">
      {error ? (
        <View className="px-4 py-3">
          <Text variant="error">{error}</Text>
        </View>
      ) : null}
      <FlatList
        data={items}
        keyExtractor={(item) => item.id}
        contentContainerClassName="px-4 py-4 gap-3"
        refreshControl={<RefreshControl refreshing={refreshing} onRefresh={() => void load(true)} />}
        ListEmptyComponent={<Text variant="muted" className="text-center py-8">No categories found</Text>}
        renderItem={({ item }) => (
          <Card>
            <CardContent className="gap-2 pt-4">
              <View className="flex-row items-center justify-between">
                <Text variant="h3">{item.name}</Text>
                <StatusBadge active={item.isActive} />
              </View>
            </CardContent>
          </Card>
        )}
      />
    </View>
  );
}
