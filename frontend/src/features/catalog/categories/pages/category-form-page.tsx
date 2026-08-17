import { useEffect } from 'react';
import { useForm, Controller } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { useTranslation } from 'react-i18next';
import { Link, useNavigate, useParams } from 'react-router';
import { toast } from 'sonner';
import { Button } from '@/shared/components/ui/button';
import { Input } from '@/shared/components/ui/input';
import { Label } from '@/shared/components/ui/label';
import { Card, CardContent, CardHeader, CardTitle } from '@/shared/components/ui/card';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/shared/components/ui/select';
import { FormFieldError } from '@/shared/components/form-field-error';
import { useGetLanguagesQuery } from '@/features/localization/api/localization-api';
import {
  useCreateCategoryMutation,
  useGetCategoriesQuery,
  useGetCategoryQuery,
  useUpdateCategoryMutation,
} from '@/features/catalog/categories/api/categories-api';
import {
  createCategorySchema,
  updateCategorySchema,
  type CreateCategoryFormValues,
  type UpdateCategoryFormValues,
} from '@/features/catalog/categories/schemas/category-schema';
import { showApiError } from '@/shared/api/show-api-error';

const EMPTY_GUID = '00000000-0000-0000-0000-000000000000';

export function CategoryFormPage() {
  const { t, i18n } = useTranslation();
  const navigate = useNavigate();
  const { id } = useParams();
  const isEdit = Boolean(id);

  const { data: categoryData, isLoading, isError, refetch } = useGetCategoryQuery(id!, { skip: !isEdit });
  const { data: categoriesData } = useGetCategoriesQuery();
  const { data: languagesData } = useGetLanguagesQuery();
  const [createCategory] = useCreateCategoryMutation();
  const [updateCategory] = useUpdateCategoryMutation();

  const languages = languagesData?.data ?? [];
  const categories = (categoriesData?.data ?? []).filter((c) => c.id !== id);
  const cultureCode = (localStorage.getItem('culture') ?? i18n.language ?? 'en').split('-')[0];
  const currentLanguageId = languages.find((l) => l.code === cultureCode)?.id ?? '';

  const createForm = useForm<CreateCategoryFormValues>({
    resolver: zodResolver(createCategorySchema),
    defaultValues: { name: '', description: '', languageId: '', parentCategoryId: '' },
  });

  const editForm = useForm<UpdateCategoryFormValues>({
    resolver: zodResolver(updateCategorySchema),
    defaultValues: { name: '', description: '', languageId: '', parentCategoryId: '', isActive: true },
  });

  useEffect(() => {
    if (!isEdit && currentLanguageId) {
      createForm.setValue('languageId', currentLanguageId);
    }
  }, [isEdit, currentLanguageId, createForm]);

  useEffect(() => {
    if (categoryData?.data && languages.length > 0) {
      const category = categoryData.data;
      const categoryLanguageId =
        category.languageId && category.languageId !== EMPTY_GUID ? category.languageId : '';
      const languageId = currentLanguageId || categoryLanguageId;
      const hasTranslationForCulture = Boolean(languageId) && categoryLanguageId === languageId;

      editForm.reset({
        name: hasTranslationForCulture ? category.name : '',
        description: hasTranslationForCulture ? (category.description ?? '') : '',
        languageId,
        parentCategoryId: category.parentCategoryId ?? '',
        isActive: category.isActive,
      });
    }
  }, [categoryData, editForm, languages.length, currentLanguageId]);

  const onCreate = async (values: CreateCategoryFormValues) => {
    try {
      await createCategory({
        name: values.name,
        description: values.description,
        languageId: values.languageId,
        parentCategoryId: values.parentCategoryId || null,
      }).unwrap();
      toast.success(t('Categories.Save.Success'));
      void navigate('/categories');
    } catch (error) {
      showApiError(error, t);
    }
  };

  const onUpdate = async (values: UpdateCategoryFormValues) => {
    if (!id) return;
    try {
      await updateCategory({
        id,
        name: values.name,
        description: values.description,
        languageId: values.languageId,
        parentCategoryId: values.parentCategoryId || null,
        isActive: values.isActive,
      }).unwrap();
      toast.success(t('Categories.Save.Success'));
      void navigate('/categories');
    } catch (error) {
      showApiError(error, t);
    }
  };

  if (isEdit && isLoading) {
    return <p className="text-muted-foreground">{t('Common.Loading')}</p>;
  }

  if (isEdit && isError) {
    return (
      <div className="space-y-4">
        <p className="text-destructive">{t('Common.NotFound')}</p>
        <div className="flex gap-2">
          <Button variant="outline" size="sm" onClick={() => void refetch()}>
            {t('Common.Retry')}
          </Button>
          <Button variant="outline" size="sm" asChild>
            <Link to="/categories">{t('Common.Cancel')}</Link>
          </Button>
        </div>
      </div>
    );
  }

  const renderLanguageSelect = (
    value: string,
    onChange: (value: string) => void,
    error?: { message?: string },
  ) => (
    <div className="space-y-2">
      <Label>{t('Categories.Form.Language')} *</Label>
      <Select value={value || undefined} onValueChange={onChange}>
        <SelectTrigger>
          <SelectValue placeholder={t('Categories.Form.SelectLanguage')} />
        </SelectTrigger>
        <SelectContent>
          {languages.map((lang) => (
            <SelectItem key={lang.id} value={lang.id}>
              {lang.nativeName} ({lang.code})
            </SelectItem>
          ))}
        </SelectContent>
      </Select>
      <FormFieldError message={error?.message} />
    </div>
  );

  const renderParentSelect = (
    value: string | undefined,
    onChange: (value: string) => void,
    error?: { message?: string },
  ) => (
    <div className="space-y-2">
      <Label>{t('Categories.Form.Parent')}</Label>
      <Select value={value || 'none'} onValueChange={(v) => onChange(v === 'none' ? '' : v)}>
        <SelectTrigger>
          <SelectValue placeholder={t('Categories.Form.SelectParent')} />
        </SelectTrigger>
        <SelectContent>
          <SelectItem value="none">{t('Categories.Form.NoParent')}</SelectItem>
          {categories.map((cat) => (
            <SelectItem key={cat.id} value={cat.id}>
              {cat.name}
            </SelectItem>
          ))}
        </SelectContent>
      </Select>
      <FormFieldError message={error?.message} />
    </div>
  );

  return (
    <div className="mx-auto max-w-2xl space-y-6">
      <h1 className="text-2xl font-bold">
        {isEdit ? t('Categories.Form.EditTitle') : t('Categories.Form.CreateTitle')}
      </h1>

      <Card>
        <CardHeader>
          <CardTitle>{isEdit ? t('Categories.Form.EditTitle') : t('Categories.Form.CreateTitle')}</CardTitle>
        </CardHeader>
        <CardContent>
          {isEdit ? (
            <form onSubmit={editForm.handleSubmit(onUpdate)} className="space-y-4">
              <div className="space-y-2">
                <Label htmlFor="name">{t('Categories.Form.Name')} *</Label>
                <Input id="name" {...editForm.register('name')} />
                <FormFieldError message={editForm.formState.errors.name?.message} />
              </div>
              <div className="space-y-2">
                <Label htmlFor="description">{t('Categories.Form.Description')}</Label>
                <Input id="description" {...editForm.register('description')} />
              </div>
              <Controller
                control={editForm.control}
                name="languageId"
                render={({ field, fieldState }) => renderLanguageSelect(field.value, field.onChange, fieldState.error)}
              />
              <Controller
                control={editForm.control}
                name="parentCategoryId"
                render={({ field, fieldState }) => renderParentSelect(field.value, field.onChange, fieldState.error)}
              />
              <div className="flex items-center gap-2">
                <input id="isActive" type="checkbox" {...editForm.register('isActive')} />
                <Label htmlFor="isActive">{t('Categories.Form.IsActive')}</Label>
              </div>
              <div className="flex gap-2">
                <Button type="submit" disabled={editForm.formState.isSubmitting}>
                  {t('Categories.Form.Save')}
                </Button>
                <Button variant="outline" asChild>
                  <Link to="/categories">{t('Categories.Form.Cancel')}</Link>
                </Button>
              </div>
            </form>
          ) : (
            <form onSubmit={createForm.handleSubmit(onCreate)} className="space-y-4">
              <div className="space-y-2">
                <Label htmlFor="name">{t('Categories.Form.Name')} *</Label>
                <Input id="name" {...createForm.register('name')} />
                <FormFieldError message={createForm.formState.errors.name?.message} />
              </div>
              <div className="space-y-2">
                <Label htmlFor="description">{t('Categories.Form.Description')}</Label>
                <Input id="description" {...createForm.register('description')} />
              </div>
              <Controller
                control={createForm.control}
                name="languageId"
                render={({ field, fieldState }) => renderLanguageSelect(field.value, field.onChange, fieldState.error)}
              />
              <Controller
                control={createForm.control}
                name="parentCategoryId"
                render={({ field, fieldState }) => renderParentSelect(field.value, field.onChange, fieldState.error)}
              />
              <div className="flex gap-2">
                <Button type="submit" disabled={createForm.formState.isSubmitting}>
                  {t('Categories.Form.Save')}
                </Button>
                <Button variant="outline" asChild>
                  <Link to="/categories">{t('Categories.Form.Cancel')}</Link>
                </Button>
              </div>
            </form>
          )}
        </CardContent>
      </Card>
    </div>
  );
}
