import { useTranslation } from 'react-i18next';
import { i18n } from '@/shared/i18n';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/shared/components/ui/select';
import { useGetLanguagesQuery } from '@/features/localization/api/localization-api';
import { useAppDispatch } from '@/app/hooks';
import { baseApi } from '@/shared/api/base-api';

export function LanguageSwitcher() {
  const { i18n: i18nInstance } = useTranslation();
  const dispatch = useAppDispatch();
  const { data } = useGetLanguagesQuery();
  const languages = data?.data ?? [];

  const handleChange = async (culture: string) => {
    localStorage.setItem('culture', culture);
    await i18n.changeLanguage(culture);
    await i18n.reloadResources(culture);
    await i18nInstance.changeLanguage(culture);
    dispatch(baseApi.util.invalidateTags(['Products', 'Product', 'Categories', 'Category', 'Languages']));
  };

  const current = languages.some((l) => l.code === i18nInstance.language)
    ? i18nInstance.language
    : (languages[0]?.code ?? 'en');

  return (
    <Select value={current} onValueChange={(value) => void handleChange(value)}>
      <SelectTrigger className="w-[140px]">
        <SelectValue />
      </SelectTrigger>
      <SelectContent>
        {languages.map((language) => (
          <SelectItem key={language.id} value={language.code}>
            {language.nativeName}
          </SelectItem>
        ))}
      </SelectContent>
    </Select>
  );
}
