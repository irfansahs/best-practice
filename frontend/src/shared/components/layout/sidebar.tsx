import { NavLink } from 'react-router';
import { Building2, FolderTree, Languages, Package, Shield, Users } from 'lucide-react';
import { useTranslation } from 'react-i18next';
import { cn } from '@/shared/lib/utils';
import { Permissions } from '@/shared/api/api-types';
import { usePermission } from '@/features/auth/hooks/use-permission';

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
    to: '/settings/organizations',
    labelKey: 'Nav.Organizations',
    icon: Building2,
    permission: Permissions.Tenancy.Organizations.Read,
  },
  {
    to: '/settings/members',
    labelKey: 'Nav.Members',
    icon: Users,
    permission: Permissions.Tenancy.Members.Read,
  },
  {
    to: '/settings/roles',
    labelKey: 'Nav.Roles',
    icon: Shield,
    permission: Permissions.Tenancy.Roles.Read,
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

  return (
    <aside className="flex w-64 flex-col border-r bg-card">
      <div className="flex h-14 items-center border-b px-6">
        <span className="text-lg font-semibold">{t('App.Title')}</span>
      </div>
      <nav className="flex flex-1 flex-col gap-1 p-4">
        {navItems.map((item) => (
          <SidebarLink key={item.to} {...item} label={t(item.labelKey)} />
        ))}
      </nav>
    </aside>
  );
}

function SidebarLink({
  to,
  label,
  icon: Icon,
  permission,
}: {
  to: string;
  label: string;
  icon: typeof Package;
  permission: string;
}) {
  const allowed = usePermission(permission);
  if (!allowed) return null;

  return (
    <NavLink
      to={to}
      className={({ isActive }) =>
        cn(
          'flex items-center gap-3 rounded-md px-3 py-2 text-sm font-medium transition-colors hover:bg-accent',
          isActive && 'bg-accent text-accent-foreground',
        )
      }
    >
      <Icon className="h-4 w-4" />
      {label}
    </NavLink>
  );
}
