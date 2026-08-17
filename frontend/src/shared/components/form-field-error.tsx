import { useTranslation } from 'react-i18next';

export function FormFieldError({ message }: { message?: string }) {
  const { t } = useTranslation();
  if (!message) return null;
  return <p className="text-sm text-destructive">{t(message)}</p>;
}
