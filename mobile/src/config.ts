// Override for Android emulator: EXPO_PUBLIC_API_URL=http://10.0.2.2:5202/api/v1
export const API_BASE_URL = process.env.EXPO_PUBLIC_API_URL ?? 'http://localhost:5202/api/v1';

export const REFRESH_TOKEN_KEY = 'refreshToken';
export const CULTURE_KEY = 'culture';

export const DEFAULT_CULTURE = 'en';
