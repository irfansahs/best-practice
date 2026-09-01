import * as SecureStore from 'expo-secure-store';
import { CULTURE_KEY, DEFAULT_CULTURE } from '@/config';

let culture = DEFAULT_CULTURE;

export function getCulture() {
  return culture;
}

export async function loadCulture() {
  const stored = await SecureStore.getItemAsync(CULTURE_KEY);
  culture = stored ?? DEFAULT_CULTURE;
  return culture;
}

export async function setCulture(value: string) {
  culture = value;
  await SecureStore.setItemAsync(CULTURE_KEY, value);
}
