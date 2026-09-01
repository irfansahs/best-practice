import { useCallback, useEffect, useState } from 'react';
import { ActivityIndicator, FlatList, RefreshControl, View } from 'react-native';
import { getApiErrorMessage } from '@/api/client';
import { getCategories } from '@/api/categories-api';
import type { CategoryListItem } from '@/api/types';
import { Card, CardContent } from '@/components/ui/card';
import { StatusBadge } from '@/components/ui/badge';
import { Text } from '@/components/ui/text';

export function CategoriesScreen() {
  const [items, setItems] = useState<CategoryListItem[]>([]);
  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const load = useCallback(async (isRefresh = false) => {
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
  }, []);

  useEffect(() => {
    void load();
  }, [load]);

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
        <View className="px-4 py-2">
          <Text variant="error">{error}</Text>
        </View>
      ) : null}
      <FlatList
        data={items}
        keyExtractor={(item) => item.id}
        contentContainerClassName="p-4 gap-3"
        refreshControl={<RefreshControl refreshing={refreshing} onRefresh={() => void load(true)} />}
        ListEmptyComponent={
          !loading ? <Text variant="muted" className="text-center py-8">No categories</Text> : null
        }
        renderItem={({ item }) => (
          <Card>
            <CardContent className="flex-row items-center justify-between pt-4">
              <Text variant="h3">{item.name}</Text>
              <StatusBadge active={item.isActive} />
            </CardContent>
          </Card>
        )}
      />
    </View>
  );
}
