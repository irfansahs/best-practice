import { z } from 'zod';

export const loginSchema = z.object({
  email: z.email('Validation.Required'),
  password: z.string().min(1, 'Validation.Required'),
});

export type LoginFormValues = z.infer<typeof loginSchema>;
