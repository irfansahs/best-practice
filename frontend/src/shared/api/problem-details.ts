export interface ProblemDetails {
  type?: string;
  title?: string;
  status?: number;
  detail?: string;
  instance?: string;
  traceId?: string;
  code?: string;
  errors?: Record<string, string[]>;
  extensions?: Record<string, unknown>;
}

export function isProblemDetails(value: unknown): value is ProblemDetails {
  return (
    typeof value === 'object' &&
    value !== null &&
    ('title' in value || 'detail' in value || 'status' in value || 'type' in value)
  );
}

export function getProblemCode(problem: ProblemDetails): string | undefined {
  const fromExtensions = problem.extensions?.code;
  if (typeof fromExtensions === 'string' && fromExtensions.length > 0) return fromExtensions;
  if (typeof problem.code === 'string' && problem.code.length > 0) return problem.code;
  return undefined;
}

type TranslateFn = (key: string) => string;

export function getProblemMessage(problem: ProblemDetails, tOrFallback?: TranslateFn | string): string {
  const code = getProblemCode(problem);
  if (code && typeof tOrFallback === 'function') {
    return tOrFallback(code);
  }

  if (problem.detail) return problem.detail;
  if (code) return code;
  if (problem.title) return problem.title;

  if (typeof tOrFallback === 'function') return tOrFallback('Common.Error');
  return tOrFallback ?? 'An unexpected error occurred';
}

export function getValidationErrors(problem: ProblemDetails): Record<string, string[]> | undefined {
  return problem.errors ?? (problem.extensions?.errors as Record<string, string[]> | undefined);
}
