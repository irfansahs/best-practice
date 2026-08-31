import axios from 'axios';
import { useCallback, useEffect, useState } from 'react';
import {
  ActivityIndicator,
  FlatList,
  StyleSheet,
  Text,
  TextInput,
  TouchableOpacity,
  View,
} from 'react-native';
import { getProducts, login } from '../api/client';

const DEFAULT_EMAIL = 'admin@local.dev';
const DEFAULT_PASSWORD = 'ChangeMe123!';

export function ProductsScreen() {
  const [email, setEmail] = useState(DEFAULT_EMAIL);
  const [password, setPassword] = useState(DEFAULT_PASSWORD);
  const [items, setItems] = useState<Awaited<ReturnType<typeof getProducts>>['items']>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [authenticated, setAuthenticated] = useState(false);

  const loadProducts = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const result = await getProducts();
      setItems(result.items);
      setAuthenticated(true);
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

  const handleLogin = async () => {
    setLoading(true);
    setError(null);
    try {
      await login(email.trim(), password);
      await loadProducts();
    } catch (e) {
      if (axios.isAxiosError(e)) {
        const status = e.response?.status;
        const msg = (e.response?.data as { message?: string } | undefined)?.message;
        setError(msg ?? `Request failed (${status ?? 'network'})`);
      } else {
        setError(e instanceof Error ? e.message : 'Login failed');
      }
      setLoading(false);
    }
  };

  useEffect(() => {
    // Ürün listesi auth gerektirir; token yoksa login ekranında kal.
    setAuthenticated(false);
  }, []);

  if (!authenticated) {
    return (
      <View style={styles.container}>
        <Text style={styles.title}>Login</Text>
        <TextInput style={styles.input} value={email} onChangeText={setEmail} autoCapitalize="none" placeholder="Email" />
        <TextInput
          style={styles.input}
          value={password}
          onChangeText={setPassword}
          secureTextEntry
          placeholder="Password"
        />
        <TouchableOpacity style={styles.button} onPress={handleLogin} disabled={loading}>
          <Text style={styles.buttonText}>{loading ? 'Loading...' : 'Sign in'}</Text>
        </TouchableOpacity>
        {error ? <Text style={styles.error}>{error}</Text> : null}
      </View>
    );
  }

  return (
    <View style={styles.container}>
      <Text style={styles.title}>Products</Text>
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
      <TouchableOpacity style={styles.button} onPress={loadProducts} disabled={loading}>
        <Text style={styles.buttonText}>Refresh</Text>
      </TouchableOpacity>
    </View>
  );
}

const styles = StyleSheet.create({
  container: { flex: 1, padding: 16, backgroundColor: '#fff' },
  title: { fontSize: 22, fontWeight: '600', marginBottom: 12 },
  input: {
    borderWidth: 1,
    borderColor: '#ccc',
    borderRadius: 8,
    padding: 12,
    marginBottom: 8,
  },
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
