import { StatusBar } from 'expo-status-bar';
import { useCallback, useEffect, useState } from 'react';
import { ActivityIndicator, SafeAreaView, StyleSheet, View } from 'react-native';
import { bootstrapSession, setSessionExpiredHandler } from './src/api/client';
import { LoginScreen } from './src/screens/LoginScreen';
import { ProductsScreen } from './src/screens/ProductsScreen';

export default function App() {
  const [authenticated, setAuthenticated] = useState(false);
  const [bootstrapping, setBootstrapping] = useState(true);

  const handleLogout = useCallback(() => setAuthenticated(false), []);

  useEffect(() => {
    setSessionExpiredHandler(handleLogout);
    void bootstrapSession()
      .then((ok) => setAuthenticated(ok))
      .finally(() => setBootstrapping(false));
    return () => setSessionExpiredHandler(null);
  }, [handleLogout]);

  return (
    <SafeAreaView style={styles.container}>
      {bootstrapping ? (
        <View style={styles.centered}>
          <ActivityIndicator size="large" />
        </View>
      ) : authenticated ? (
        <ProductsScreen onLogout={handleLogout} />
      ) : (
        <LoginScreen onSuccess={() => setAuthenticated(true)} />
      )}
      <StatusBar style="auto" />
    </SafeAreaView>
  );
}

const styles = StyleSheet.create({
  container: { flex: 1, backgroundColor: '#fff' },
  centered: { flex: 1, justifyContent: 'center', alignItems: 'center' },
});
