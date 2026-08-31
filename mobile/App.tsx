import { StatusBar } from 'expo-status-bar';
import { SafeAreaView, StyleSheet } from 'react-native';
import { ProductsScreen } from './src/screens/ProductsScreen';

export default function App() {
  return (
    <SafeAreaView style={styles.container}>
      <ProductsScreen />
      <StatusBar style="auto" />
    </SafeAreaView>
  );
}

const styles = StyleSheet.create({
  container: { flex: 1, backgroundColor: '#fff' },
});
