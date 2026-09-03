import { useTranslation } from 'react-i18next';
import { toast } from 'sonner';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/shared/components/ui/select';
import { useAppDispatch, useAppSelector } from '@/app/hooks';
import {
  selectActiveOrganization,
  selectCurrentUser,
  selectOrganizations,
  switchOrganization,
} from '@/features/auth/slice/auth-slice';
import { showApiError } from '@/shared/api/show-api-error';

export function OrganizationSwitcher() {
  const { t } = useTranslation();
  const dispatch = useAppDispatch();
  const user = useAppSelector(selectCurrentUser);
  const active = useAppSelector(selectActiveOrganization);
  const organizations = useAppSelector(selectOrganizations);

  if (!active && organizations.length === 0) return null;

  const currentId = active?.id ?? organizations[0]?.id;
  if (!currentId) return null;

  const onChange = async (organizationId: string) => {
    if (organizationId === currentId) return;
    try {
      await dispatch(switchOrganization(organizationId)).unwrap();
      toast.success(t('Auth.Switch.Success'));
    } catch (error) {
      showApiError(error, t);
    }
  };

  return (
    <div className="flex min-w-40 max-w-64 flex-col gap-1">
      <Select value={currentId} onValueChange={(value) => void onChange(value)}>
        <SelectTrigger className="h-9">
          <SelectValue placeholder={t('Auth.Organization.Placeholder')} />
        </SelectTrigger>
        <SelectContent>
          {organizations.map((org) => (
            <SelectItem key={org.id} value={org.id}>
              {org.name}
            </SelectItem>
          ))}
        </SelectContent>
      </Select>
      {user?.isImpersonating ? (
        <span className="text-xs text-muted-foreground">{t('Auth.Impersonating')}</span>
      ) : null}
    </div>
  );
}
