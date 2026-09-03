import { useCallback, useEffect, useState } from 'react';
import { ActivityIndicator, ScrollView, View } from 'react-native';
import { useRoute, type RouteProp } from '@react-navigation/native';
import { getApiErrorMessage } from '@/api/client';
import { getProductById } from '@/api/products-api';
import type { ProductDetailDto } from '@/api/types';
import { Permissions } from '@/api/types';
import type { AppStackParamList } from '@/navigation/types';
import { useAuth } from '@/contexts/AuthContext';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { StatusBadge } from '@/components/ui/badge';
import { Text } from '@/components/ui/text';

export function ProductDetailScreen() {
  const route = useRoute<RouteProp<AppStackParamList, 'ProductDetail'>>();
  const { id } = route.params;
  const { hasPermission } = useAuth();
  const canRead = hasPermission(Permissions.Catalog.Products.Read);
  const [product, setProduct] = useState<ProductDetailDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const load = useCallback(async () => {
    if (!canRead) return;
    setLoading(true);
    setError(null);
    try {
      const data = await getProductById(id);
      setProduct(data);
    } catch (e) {
      setError(getApiErrorMessage(e));
      setProduct(null);
    } finally {
      setLoading(false);
    }
  }, [id, canRead]);

  useEffect(() => {
    void load();
  }, [load]);

  if (!canRead) {
    return (
      <View className="flex-1 items-center justify-center bg-background px-4">
        <Text variant="muted" className="text-center">
          You do not have permission to view this product.
        </Text>
      </View>
    );
  }

  if (loading) {
    return (
      <View className="flex-1 items-center justify-center bg-background">
        <ActivityIndicator size="large" color="#5b5bd6" />
      </View>
    );
  }

  if (error || !product) {
    return (
      <View className="flex-1 items-center justify-center bg-background p-4">
        <Text variant="error">{error ?? 'Product not found'}</Text>
      </View>
    );
  }

  return (
    <ScrollView className="flex-1 bg-background" contentContainerClassName="p-4">
      <Card>
        <CardHeader>
          <View className="flex-row items-center justify-between">
            <CardTitle>{product.name}</CardTitle>
            <StatusBadge active={product.isActive} />
          </View>
        </CardHeader>
        <CardContent className="gap-3">
          <View>
            <Text variant="label">SKU</Text>
            <Text className="font-mono">{product.sku}</Text>
          </View>
          <View>
            <Text variant="label">Price</Text>
            <Text>{product.price.toFixed(2)} {product.currency}</Text>
          </View>
          <View>
            <Text variant="label">Slug</Text>
            <Text variant="muted">{product.slug}</Text>
          </View>
          {product.description ? (
            <View>
              <Text variant="label">Description</Text>
              <Text>{product.description}</Text>
            </View>
          ) : null}
        </CardContent>
      </Card>
    </ScrollView>
  );
}
