import { baseApi } from '@/shared/api/base-api';
import type {
  ApiResponse,
  ImportTranslationsResponse,
  Language,
  UpsertTranslationResponse,
} from '@/shared/api/api-types';

export const localizationApi = baseApi.injectEndpoints({
  endpoints: (builder) => ({
    getLanguages: builder.query<ApiResponse<Language[]>, void>({
      query: () => ({
        url: '/localization/languages',
      }),
      providesTags: ['Languages'],
    }),
    upsertTranslation: builder.mutation<
      ApiResponse<UpsertTranslationResponse>,
      { languageId: string; namespace: string; key: string; value: string }
    >({
      query: (body) => ({
        url: '/localization/translations',
        method: 'PUT',
        data: body,
      }),
      invalidatesTags: ['Resources'],
    }),
    importTranslations: builder.mutation<
      ApiResponse<ImportTranslationsResponse>,
      {
        items: Array<{ languageId: string; namespace: string; key: string; value: string }>;
      }
    >({
      query: (body) => ({
        url: '/localization/translations/import',
        method: 'POST',
        data: body,
      }),
      invalidatesTags: ['Resources'],
    }),
  }),
});

export const { useGetLanguagesQuery, useUpsertTranslationMutation, useImportTranslationsMutation } = localizationApi;
