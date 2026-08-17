import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { useTranslation } from 'react-i18next';
import { Navigate, useLocation } from 'react-router';
import { toast } from 'sonner';
import { Button } from '@/shared/components/ui/button';
import { Input } from '@/shared/components/ui/input';
import { Label } from '@/shared/components/ui/label';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/shared/components/ui/card';
import { FormFieldError } from '@/shared/components/form-field-error';
import { useAppDispatch, useAppSelector } from '@/app/hooks';
import { login, selectHasPermission, selectIsAuthenticated } from '@/features/auth/slice/auth-slice';
import { loginSchema, type LoginFormValues } from '@/features/auth/schemas/login-schema';
import { Permissions } from '@/shared/api/api-types';

export function LoginPage() {
  const { t } = useTranslation();
  const dispatch = useAppDispatch();
  const isAuthenticated = useAppSelector(selectIsAuthenticated);
  const canReadProducts = useAppSelector(selectHasPermission(Permissions.Catalog.Products.Read));
  const canReadCategories = useAppSelector(selectHasPermission(Permissions.Catalog.Categories.Read));
  const location = useLocation();
  const from = (location.state as { from?: { pathname?: string } } | null)?.from?.pathname;

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<LoginFormValues>({
    resolver: zodResolver(loginSchema),
    defaultValues: { email: '', password: '' },
  });

  if (isAuthenticated) {
    const fallback = canReadProducts ? '/products' : canReadCategories ? '/categories' : '/';
    return <Navigate to={from && from !== '/login' ? from : fallback} replace />;
  }

  const onSubmit = async (values: LoginFormValues) => {
    try {
      await dispatch(login(values)).unwrap();
      toast.success(t('Auth.Login.Success'));
    } catch (error) {
      const key = typeof error === 'string' ? error : 'Auth.Login.Error';
      toast.error(t(key));
    }
  };

  return (
    <div className="flex min-h-screen items-center justify-center bg-muted/30 p-4">
      <Card className="w-full max-w-md">
        <CardHeader>
          <CardTitle>{t('Auth.Login.Title')}</CardTitle>
          <CardDescription>{t('App.Title')}</CardDescription>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
            <div className="space-y-2">
              <Label htmlFor="email">{t('Auth.Login.Email')}</Label>
              <Input id="email" type="email" autoComplete="email" {...register('email')} />
              <FormFieldError message={errors.email?.message} />
            </div>
            <div className="space-y-2">
              <Label htmlFor="password">{t('Auth.Login.Password')}</Label>
              <Input id="password" type="password" autoComplete="current-password" {...register('password')} />
              <FormFieldError message={errors.password?.message} />
            </div>
            <Button type="submit" className="w-full" disabled={isSubmitting}>
              {isSubmitting ? t('Common.Loading') : t('Auth.Login.Submit')}
            </Button>
          </form>
        </CardContent>
      </Card>
    </div>
  );
}
