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
import {
  useChangeProductPriceMutation,
  useCreateProductMutation,
  useGetProductQuery,
  useUpdateProductMutation,
} from '@/features/catalog/products/api/products-api';
import { useGetCategoriesQuery } from '@/features/catalog/categories/api/categories-api';
import { useGetLanguagesQuery } from '@/features/localization/api/localization-api';
import {
  createProductSchema,
  updateProductSchema,
  type CreateProductFormValues,
  type UpdateProductFormValues,
} from '@/features/catalog/products/schemas/product-schema';
import { showApiError } from '@/shared/api/show-api-error';

const EMPTY_GUID = '00000000-0000-0000-0000-000000000000';

export function ProductFormPage() {
  const { t, i18n } = useTranslation();
  const navigate = useNavigate();
  const { id } = useParams();
  const isEdit = Boolean(id);

  const { data: productData, isLoading, isError, refetch } = useGetProductQuery(id!, { skip: !isEdit });
  const { data: categoriesData } = useGetCategoriesQuery();
  const { data: languagesData } = useGetLanguagesQuery();
  const [createProduct] = useCreateProductMutation();
  const [updateProduct] = useUpdateProductMutation();
  const [changePrice] = useChangeProductPriceMutation();

  const categories = categoriesData?.data ?? [];
  const languages = languagesData?.data ?? [];
  const cultureCode = (localStorage.getItem('culture') ?? i18n.language ?? 'en').split('-')[0];
  const currentLanguageId = languages.find((l) => l.code === cultureCode)?.id ?? '';

  const createForm = useForm<CreateProductFormValues>({
    resolver: zodResolver(createProductSchema),
    defaultValues: {
      sku: '',
      name: '',
      description: '',
      price: 0,
      currency: 'USD',
      categoryId: '',
      languageId: '',
    },
  });

  const editForm = useForm<UpdateProductFormValues>({
    resolver: zodResolver(updateProductSchema),
    defaultValues: {
      name: '',
      description: '',
      categoryId: '',
      languageId: '',
      isActive: true,
      price: 0,
      currency: 'USD',
    },
  });

  useEffect(() => {
    if (!isEdit && currentLanguageId) {
      createForm.setValue('languageId', currentLanguageId);
    }
  }, [isEdit, currentLanguageId, createForm]);

  useEffect(() => {
    if (productData?.data && languages.length > 0) {
      const product = productData.data;
      const productLanguageId =
        product.languageId && product.languageId !== EMPTY_GUID ? product.languageId : '';
      const languageId = currentLanguageId || productLanguageId;
      const hasTranslationForCulture = Boolean(languageId) && productLanguageId === languageId;

      editForm.reset({
        name: hasTranslationForCulture ? product.name : '',
        description: hasTranslationForCulture ? (product.description ?? '') : '',
        categoryId: product.categoryId,
        languageId,
        isActive: product.isActive,
        price: product.price,
        currency: product.currency,
      });
    }
  }, [productData, editForm, languages.length, currentLanguageId]);

  const onCreate = async (values: CreateProductFormValues) => {
    try {
      await createProduct(values).unwrap();
      toast.success(t('Products.Save.Success'));
      void navigate('/products');
    } catch (error) {
      showApiError(error, t);
    }
  };

  const onUpdate = async (values: UpdateProductFormValues) => {
    if (!id) return;
    try {
      await updateProduct({
        id,
        categoryId: values.categoryId,
        languageId: values.languageId,
        name: values.name,
        description: values.description,
        isActive: values.isActive,
      }).unwrap();

      const loaded = productData?.data;
      const priceChanged =
        loaded && (loaded.price !== values.price || loaded.currency !== values.currency);

      if (priceChanged) {
        await changePrice({ id, price: values.price, currency: values.currency }).unwrap();
      }

      toast.success(t('Products.Save.Success'));
      void navigate('/products');
    } catch (error) {
      showApiError(error, t);
    }
  };

  const renderCategorySelect = (
    value: string,
    onChange: (value: string) => void,
    error?: { message?: string },
  ) => (
    <div className="space-y-2">
      <Label>{t('Products.Form.Category')} *</Label>
      <Select value={value || undefined} onValueChange={onChange}>
        <SelectTrigger>
          <SelectValue placeholder={t('Products.Form.SelectCategory')} />
        </SelectTrigger>
        <SelectContent>
          {categories.map((category) => (
            <SelectItem key={category.id} value={category.id}>
              {category.name}
            </SelectItem>
          ))}
        </SelectContent>
      </Select>
      <FormFieldError message={error?.message} />
    </div>
  );

  const renderLanguageSelect = (
    value: string,
    onChange: (value: string) => void,
    error?: { message?: string },
  ) => (
    <div className="space-y-2">
      <Label>{t('Products.Form.Language')} *</Label>
      <Select value={value || undefined} onValueChange={onChange}>
        <SelectTrigger>
          <SelectValue placeholder={t('Products.Form.SelectLanguage')} />
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
            <Link to="/products">{t('Common.Cancel')}</Link>
          </Button>
        </div>
      </div>
    );
  }

  return (
    <div className="mx-auto max-w-2xl space-y-6">
      <h1 className="text-2xl font-bold">
        {isEdit ? t('Products.Form.EditTitle') : t('Products.Form.CreateTitle')}
      </h1>

      <Card>
        <CardHeader>
          <CardTitle>{isEdit ? t('Products.Form.EditTitle') : t('Products.Form.CreateTitle')}</CardTitle>
        </CardHeader>
        <CardContent>
          {isEdit ? (
            <form onSubmit={editForm.handleSubmit(onUpdate)} className="space-y-4">
              <div className="space-y-2">
                <Label htmlFor="name">{t('Products.Form.Name')} *</Label>
                <Input id="name" {...editForm.register('name')} />
                <FormFieldError message={editForm.formState.errors.name?.message} />
              </div>
              <div className="space-y-2">
                <Label htmlFor="description">{t('Products.Form.Description')}</Label>
                <Input id="description" {...editForm.register('description')} />
              </div>
              <div className="grid grid-cols-2 gap-4">
                <div className="space-y-2">
                  <Label htmlFor="price">{t('Products.Form.Price')} *</Label>
                  <Input id="price" type="number" step="0.01" {...editForm.register('price', { valueAsNumber: true })} />
                  <FormFieldError message={editForm.formState.errors.price?.message} />
                </div>
                <div className="space-y-2">
                  <Label htmlFor="currency">{t('Products.Form.Currency')} *</Label>
                  <Input id="currency" maxLength={3} {...editForm.register('currency')} />
                  <FormFieldError message={editForm.formState.errors.currency?.message} />
                </div>
              </div>
              <Controller
                control={editForm.control}
                name="categoryId"
                render={({ field, fieldState }) => renderCategorySelect(field.value, field.onChange, fieldState.error)}
              />
              <Controller
                control={editForm.control}
                name="languageId"
                render={({ field, fieldState }) => renderLanguageSelect(field.value, field.onChange, fieldState.error)}
              />
              <div className="flex items-center gap-2">
                <input id="isActive" type="checkbox" {...editForm.register('isActive')} />
                <Label htmlFor="isActive">{t('Products.Form.IsActive')}</Label>
              </div>
              <div className="flex gap-2">
                <Button type="submit" disabled={editForm.formState.isSubmitting}>
                  {t('Products.Form.Save')}
                </Button>
                <Button variant="outline" asChild>
                  <Link to="/products">{t('Products.Form.Cancel')}</Link>
                </Button>
              </div>
            </form>
          ) : (
            <form onSubmit={createForm.handleSubmit(onCreate)} className="space-y-4">
              <div className="space-y-2">
                <Label htmlFor="sku">{t('Products.Form.Sku')} *</Label>
                <Input id="sku" {...createForm.register('sku')} />
                <FormFieldError message={createForm.formState.errors.sku?.message} />
              </div>
              <div className="space-y-2">
                <Label htmlFor="name">{t('Products.Form.Name')} *</Label>
                <Input id="name" {...createForm.register('name')} />
                <FormFieldError message={createForm.formState.errors.name?.message} />
              </div>
              <div className="space-y-2">
                <Label htmlFor="description">{t('Products.Form.Description')}</Label>
                <Input id="description" {...createForm.register('description')} />
              </div>
              <div className="grid grid-cols-2 gap-4">
                <div className="space-y-2">
                  <Label htmlFor="price">{t('Products.Form.Price')} *</Label>
                  <Input id="price" type="number" step="0.01" {...createForm.register('price', { valueAsNumber: true })} />
                  <FormFieldError message={createForm.formState.errors.price?.message} />
                </div>
                <div className="space-y-2">
                  <Label htmlFor="currency">{t('Products.Form.Currency')} *</Label>
                  <Input id="currency" maxLength={3} {...createForm.register('currency')} />
                  <FormFieldError message={createForm.formState.errors.currency?.message} />
                </div>
              </div>
              <Controller
                control={createForm.control}
                name="categoryId"
                render={({ field, fieldState }) => renderCategorySelect(field.value, field.onChange, fieldState.error)}
              />
              <Controller
                control={createForm.control}
                name="languageId"
                render={({ field, fieldState }) => renderLanguageSelect(field.value, field.onChange, fieldState.error)}
              />
              <div className="flex gap-2">
                <Button type="submit" disabled={createForm.formState.isSubmitting}>
                  {t('Products.Form.Save')}
                </Button>
                <Button variant="outline" asChild>
                  <Link to="/products">{t('Products.Form.Cancel')}</Link>
                </Button>
              </div>
            </form>
          )}
        </CardContent>
      </Card>
    </div>
  );
}
