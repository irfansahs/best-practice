import { http, HttpResponse } from 'msw';
import { setupServer } from 'msw/node';

const API_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:5202/api/v1';

export const handlers = [
  http.get(`${API_URL}/localization/resources/:culture`, () =>
    HttpResponse.json({
      success: true,
      data: { culture: 'en', resources: { 'App.Title': 'Best Practice App' } },
      meta: {},
    }),
  ),
  http.post(`${API_URL}/auth/refresh`, () =>
    HttpResponse.json({
      success: true,
      data: {
        accessToken: 'test-token',
        refreshToken: 'refresh',
        expiresAt: new Date().toISOString(),
      },
      meta: {},
    }),
  ),
];

export const server = setupServer(...handlers);
