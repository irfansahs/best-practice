import { Platform } from 'react-native';
import * as SecureStore from 'expo-secure-store';
import { CULTURE_KEY, DEFAULT_CULTURE } from '@/config';

let culture = DEFAULT_CULTURE;

const isWeb = Platform.OS === 'web';

export function getCulture() {
  return culture;
}

export async function loadCulture() {
  if (isWeb) {
    culture = localStorage.getItem(CULTURE_KEY) ?? DEFAULT_CULTURE;
    return culture;
  }
  const stored = await SecureStore.getItemAsync(CULTURE_KEY);
  culture = stored ?? DEFAULT_CULTURE;
  return culture;
}

export async function setCulture(value: string) {
  culture = value;
  if (isWeb) {
    localStorage.setItem(CULTURE_KEY, value);
    return;
  }
  await SecureStore.setItemAsync(CULTURE_KEY, value);
}
