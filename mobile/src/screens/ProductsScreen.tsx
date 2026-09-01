import axios from 'axios';
import { useCallback, useEffect, useState } from 'react';
import {
  ActivityIndicator,
  FlatList,
  StyleSheet,
  Text,
  TouchableOpacity,
  View,
} from 'react-native';
import { getProducts, logout } from '../api/client';

interface ProductsScreenProps {
  onLogout: () => void;
}

export function ProductsScreen({ onLogout }: ProductsScreenProps) {
  const [items, setItems] = useState<Awaited<ReturnType<typeof getProducts>>['items']>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const loadProducts = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const result = await getProducts();
      setItems(result.items);
    } catch (e) {
      if (axios.isAxiosError(e)) {
        const status = e.response?.status;
        const msg = (e.response?.data as { message?: string } | undefined)?.message;
        setError(msg ?? `Request failed (${status ?? 'network'})`);
      } else {
        setError(e instanceof Error ? e.message : 'Request failed');
      }
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    void loadProducts();
  }, [loadProducts]);

  const handleLogout = async () => {
    await logout();
    onLogout();
  };

  return (
    <View style={styles.container}>
      <View style={styles.header}>
        <Text style={styles.title}>Products</Text>
        <TouchableOpacity onPress={() => void handleLogout()}>
          <Text style={styles.logout}>Logout</Text>
        </TouchableOpacity>
      </View>
      {loading ? <ActivityIndicator size="large" /> : null}
      {error ? <Text style={styles.error}>{error}</Text> : null}
      <FlatList
        data={items}
        keyExtractor={(item) => item.id}
        renderItem={({ item }) => (
          <View style={styles.row}>
            <Text style={styles.sku}>{item.sku}</Text>
            <Text>{item.name}</Text>
            <Text>{item.price} {item.currency}</Text>
          </View>
        )}
        ListEmptyComponent={!loading ? <Text>No products</Text> : null}
      />
      <TouchableOpacity style={styles.button} onPress={() => void loadProducts()} disabled={loading}>
        <Text style={styles.buttonText}>Refresh</Text>
      </TouchableOpacity>
    </View>
  );
}

const styles = StyleSheet.create({
  container: { flex: 1, padding: 16, backgroundColor: '#fff' },
  header: { flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center', marginBottom: 12 },
  title: { fontSize: 22, fontWeight: '600' },
  logout: { color: '#2563eb', fontWeight: '600' },
  button: {
    backgroundColor: '#2563eb',
    padding: 12,
    borderRadius: 8,
    alignItems: 'center',
    marginTop: 8,
  },
  buttonText: { color: '#fff', fontWeight: '600' },
  error: { color: '#dc2626', marginTop: 8 },
  row: {
    paddingVertical: 10,
    borderBottomWidth: 1,
    borderBottomColor: '#eee',
  },
  sku: { fontWeight: '600' },
});
