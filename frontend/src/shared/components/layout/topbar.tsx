import { useTranslation } from 'react-i18next';
import { LogOut } from 'lucide-react';
import { Button } from '@/shared/components/ui/button';
import { LanguageSwitcher } from '@/shared/components/layout/language-switcher';
import { OrganizationSwitcher } from '@/shared/components/layout/organization-switcher';
import { useAppDispatch, useAppSelector } from '@/app/hooks';
import { logout, selectCurrentUser } from '@/features/auth/slice/auth-slice';

export function Topbar() {
  const { t } = useTranslation();
  const dispatch = useAppDispatch();
  const user = useAppSelector(selectCurrentUser);

  return (
    <header className="flex h-14 items-center justify-between border-b bg-background px-6">
      <div className="text-sm text-muted-foreground">{user?.fullName ?? user?.email}</div>
      <div className="flex items-center gap-3">
        <OrganizationSwitcher />
        <LanguageSwitcher />
        <Button variant="outline" size="sm" onClick={() => void dispatch(logout())}>
          <LogOut className="mr-2 h-4 w-4" />
          {t('Nav.Logout')}
        </Button>
      </div>
    </header>
  );
}
