import { z } from 'zod';

export const createCategorySchema = z.object({
  name: z.string().min(1, 'Validation.Required'),
  description: z.string().optional(),
  languageId: z.guid('Validation.Invalid'),
  parentCategoryId: z.union([z.guid('Validation.Invalid'), z.literal('')]).optional(),
});

export const updateCategorySchema = createCategorySchema.extend({
  isActive: z.boolean(),
});

export type CreateCategoryFormValues = z.infer<typeof createCategorySchema>;
export type UpdateCategoryFormValues = z.infer<typeof updateCategorySchema>;
