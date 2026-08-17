import { useRef } from 'react';
import { useForm, Controller } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { useTranslation } from 'react-i18next';
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
import {
  useGetLanguagesQuery,
  useImportTranslationsMutation,
  useUpsertTranslationMutation,
} from '@/features/localization/api/localization-api';
import {
  upsertTranslationSchema,
  type UpsertTranslationFormValues,
} from '@/features/localization/schemas/translation-schema';
import { FormFieldError } from '@/shared/components/form-field-error';
import { showApiError } from '@/shared/api/show-api-error';
import { i18n } from '@/shared/i18n';

export function TranslationManagerPage() {
  const { t } = useTranslation();
  const fileInputRef = useRef<HTMLInputElement>(null);
  const { data: languagesData, isLoading } = useGetLanguagesQuery();
  const [upsertTranslation] = useUpsertTranslationMutation();
  const [importTranslations] = useImportTranslationsMutation();

  const {
    register,
    handleSubmit,
    control,
    watch,
    formState: { isSubmitting, errors },
  } = useForm<UpsertTranslationFormValues>({
    resolver: zodResolver(upsertTranslationSchema),
    defaultValues: {
      languageId: '',
      namespace: 'translation',
      key: '',
      value: '',
    },
  });

  const languageId = watch('languageId');
  const languages = languagesData?.data ?? [];

  const onSubmit = async (values: UpsertTranslationFormValues) => {
    try {
      await upsertTranslation(values).unwrap();
      await i18n.reloadResources(i18n.language);
      toast.success(t('Localization.Save.Success'));
    } catch (error) {
      showApiError(error, t);
    }
  };

  const handleImport = async (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (!file || !languageId) {
      toast.error(t('Validation.Required'));
      return;
    }

    try {
      const text = await file.text();
      const parsed = JSON.parse(text) as Record<string, string>;
      const items = Object.entries(parsed).map(([key, value]) => ({
        languageId,
        namespace: 'translation',
        key,
        value,
      }));
      const result = await importTranslations({ items }).unwrap();
      await i18n.reloadResources(i18n.language);
      toast.success(t('Localization.Import.Success', { count: result.data.importedCount }));
    } catch (error) {
      showApiError(error, t);
    } finally {
      if (fileInputRef.current) fileInputRef.current.value = '';
    }
  };

  if (isLoading) {
    return <p className="text-muted-foreground">{t('Common.Loading')}</p>;
  }

  return (
    <div className="mx-auto max-w-2xl space-y-6">
      <h1 className="text-2xl font-bold">{t('Localization.Title')}</h1>

      <Card>
        <CardHeader>
          <CardTitle>{t('Localization.Title')}</CardTitle>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
            <div className="space-y-2">
              <Label>{t('Localization.Language')}</Label>
              <Controller
                control={control}
                name="languageId"
                render={({ field }) => (
                  <Select value={field.value || undefined} onValueChange={field.onChange}>
                    <SelectTrigger>
                      <SelectValue placeholder={t('Localization.Language')} />
                    </SelectTrigger>
                    <SelectContent>
                      {languages.map((language) => (
                        <SelectItem key={language.id} value={language.id}>
                          {language.nativeName} ({language.code})
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                )}
              />
              <FormFieldError message={errors.languageId?.message} />
            </div>
            <div className="space-y-2">
              <Label htmlFor="namespace">{t('Localization.Namespace')}</Label>
              <Input id="namespace" {...register('namespace')} />
              <FormFieldError message={errors.namespace?.message} />
            </div>
            <div className="space-y-2">
              <Label htmlFor="key">{t('Localization.Key')}</Label>
              <Input id="key" {...register('key')} />
              <FormFieldError message={errors.key?.message} />
            </div>
            <div className="space-y-2">
              <Label htmlFor="value">{t('Localization.Value')}</Label>
              <Input id="value" {...register('value')} />
              <FormFieldError message={errors.value?.message} />
            </div>
            <div className="flex gap-2">
              <Button type="submit" disabled={isSubmitting}>
                {t('Localization.Save')}
              </Button>
              <Button type="button" variant="outline" onClick={() => fileInputRef.current?.click()}>
                {t('Localization.Import')}
              </Button>
              <input ref={fileInputRef} type="file" accept="application/json" className="hidden" onChange={handleImport} />
            </div>
          </form>
        </CardContent>
      </Card>
    </div>
  );
}
