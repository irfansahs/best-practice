import { Platform } from 'react-native';
import * as SecureStore from 'expo-secure-store';
import { REFRESH_TOKEN_KEY } from '@/config';

let accessToken: string | null = null;

const isWeb = Platform.OS === 'web';

export function getAccessToken() {
  return accessToken;
}

export function setAccessToken(token: string | null) {
  accessToken = token;
}

export async function getRefreshToken() {
  if (isWeb) return localStorage.getItem(REFRESH_TOKEN_KEY);
  return await SecureStore.getItemAsync(REFRESH_TOKEN_KEY);
}

export async function setRefreshToken(token: string) {
  if (isWeb) {
    localStorage.setItem(REFRESH_TOKEN_KEY, token);
    return;
  }
  await SecureStore.setItemAsync(REFRESH_TOKEN_KEY, token);
}

export async function clearTokens() {
  accessToken = null;
  if (isWeb) {
    localStorage.removeItem(REFRESH_TOKEN_KEY);
    return;
  }
  await SecureStore.deleteItemAsync(REFRESH_TOKEN_KEY);
}
