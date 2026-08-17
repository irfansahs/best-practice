import { z } from 'zod';

export const createProductSchema = z.object({
  sku: z.string().min(1, 'Validation.Required'),
  name: z.string().min(1, 'Validation.Required'),
  description: z.string().optional(),
  price: z.number().min(0, 'Validation.MinZero'),
  currency: z.string().length(3, 'Validation.Length'),
  categoryId: z.guid('Validation.Invalid'),
  languageId: z.guid('Validation.Invalid'),
});

export const updateProductSchema = z.object({
  name: z.string().min(1, 'Validation.Required'),
  description: z.string().optional(),
  categoryId: z.guid('Validation.Invalid'),
  languageId: z.guid('Validation.Invalid'),
  isActive: z.boolean(),
  price: z.number().min(0, 'Validation.MinZero'),
  currency: z.string().length(3, 'Validation.Length'),
});

export type CreateProductFormValues = z.infer<typeof createProductSchema>;
export type UpdateProductFormValues = z.infer<typeof updateProductSchema>;
