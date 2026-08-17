import { z } from 'zod';

export const upsertTranslationSchema = z.object({
  languageId: z.guid('Validation.Invalid'),
  namespace: z.string().min(1, 'Validation.Required'),
  key: z.string().min(1, 'Validation.Required'),
  value: z.string().min(1, 'Validation.Required'),
});

export type UpsertTranslationFormValues = z.infer<typeof upsertTranslationSchema>;
