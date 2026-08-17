import { NavLink } from 'react-router';
import { FolderTree, Package, Languages } from 'lucide-react';
import { useTranslation } from 'react-i18next';
import { cn } from '@/shared/lib/utils';
import { Permissions } from '@/shared/api/api-types';
import { useAppSelector } from '@/app/hooks';

const navItems = [
  {
    to: '/products',
    labelKey: 'Nav.Products',
    icon: Package,
    permission: Permissions.Catalog.Products.Read,
  },
  {
    to: '/categories',
    labelKey: 'Nav.Categories',
    icon: FolderTree,
    permission: Permissions.Catalog.Categories.Read,
  },
  {
    to: '/localization',
    labelKey: 'Nav.Localization',
    icon: Languages,
    permission: Permissions.Localization.Manage,
  },
];

export function Sidebar() {
  const { t } = useTranslation();
  const permissions = useAppSelector((state) => state.auth.user?.permissions ?? []);

  return (
    <aside className="flex w-64 flex-col border-r bg-card">
      <div className="flex h-14 items-center border-b px-6">
        <span className="text-lg font-semibold">{t('App.Title')}</span>
      </div>
      <nav className="flex flex-1 flex-col gap-1 p-4">
        {navItems
          .filter((item) => permissions.includes(item.permission))
          .map((item) => (
            <NavLink
              key={item.to}
              to={item.to}
              className={({ isActive }) =>
                cn(
                  'flex items-center gap-3 rounded-md px-3 py-2 text-sm font-medium transition-colors hover:bg-accent',
                  isActive && 'bg-accent text-accent-foreground',
                )
              }
            >
              <item.icon className="h-4 w-4" />
              {t(item.labelKey)}
            </NavLink>
          ))}
      </nav>
    </aside>
  );
}
