import axios from 'axios';
import type { BackendModule, ReadCallback, Services } from 'i18next';
import type { ApiResponse, ResourceBundle } from '@/shared/api/api-types';

const API_BASE_URL = import.meta.env.VITE_API_URL;

export class DbBackend implements BackendModule {
  static type = 'backend' as const;

  type = 'backend' as const;

  init(_services: Services, _options: object) {
    // no-op
  }

  read(language: string, _namespace: string, callback: ReadCallback) {
    axios
      .get<ApiResponse<ResourceBundle>>(`${API_BASE_URL}/localization/resources/${language}`, {
        withCredentials: false,
      })
      .then((response) => {
        callback(null, response.data.data.resources);
      })
      .catch((error: unknown) => {
        callback(error instanceof Error ? error : new Error('Failed to load translations'), null);
      });
  }
}
