import { useTranslation } from 'react-i18next';
import { Card, CardContent, CardHeader, CardTitle } from '@/shared/components/ui/card';

export function AccessDenied() {
  const { t } = useTranslation();

  return (
    <div className="flex flex-1 items-center justify-center p-6">
      <Card className="max-w-md">
        <CardHeader>
          <CardTitle>{t('Common.AccessDenied.Title')}</CardTitle>
        </CardHeader>
        <CardContent>
          <p className="text-muted-foreground">{t('Common.AccessDenied.Message')}</p>
        </CardContent>
      </Card>
    </div>
  );
}
