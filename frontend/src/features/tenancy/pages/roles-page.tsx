import { useMemo, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { toast } from 'sonner';
import { Button } from '@/shared/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/shared/components/ui/card';
import { Input } from '@/shared/components/ui/input';
import { Label } from '@/shared/components/ui/label';
import {
  Dialog,
  DialogContent,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/shared/components/ui/dialog';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/shared/components/ui/select';
import { PermissionScope, Permissions, type PermissionCatalogItem } from '@/shared/api/api-types';
import { Can } from '@/shared/components/can';
import { showApiError } from '@/shared/api/show-api-error';
import { useAppSelector } from '@/app/hooks';
import { selectActiveOrganization } from '@/features/auth/slice/auth-slice';
import {
  useCreateRoleMutation,
  useGetPermissionCatalogQuery,
  useGetRolesQuery,
  useUpdateRolePermissionsMutation,
} from '@/features/tenancy/api/tenancy-api';

const scopeLabels = ['Own', 'Organization', 'Subtree', 'Global'] as const;

export function RolesPage() {
  const { t } = useTranslation();
  const activeOrganization = useAppSelector(selectActiveOrganization);
  const canEditSystemRoles = activeOrganization?.type === 'Platform';
  const { data, isLoading, isError, refetch } = useGetRolesQuery();
  const { data: catalogData } = useGetPermissionCatalogQuery();
  const [createRole] = useCreateRoleMutation();
  const [updateRolePermissions] = useUpdateRolePermissionsMutation();
  const [createOpen, setCreateOpen] = useState(false);
  const [editRoleId, setEditRoleId] = useState<string | null>(null);
  const [name, setName] = useState('');
  const [description, setDescription] = useState('');
  const [allowedClients, setAllowedClients] = useState('3');
  const [grants, setGrants] = useState<Record<string, number>>({});

  const roles = data?.data ?? [];
  const catalog = catalogData?.data ?? [];
  const modules = useMemo(() => {
    const grouped = new Map<string, PermissionCatalogItem[]>();
    for (const item of catalog) {
      const list = grouped.get(item.module) ?? [];
      list.push(item);
      grouped.set(item.module, list);
    }
    return [...grouped.entries()];
  }, [catalog]);

  const editing = roles.find((r) => r.id === editRoleId);

  const openCreate = () => {
    setName('');
    setDescription('');
    setAllowedClients('3');
    setGrants({});
    setCreateOpen(true);
  };

  const openEdit = (roleId: string) => {
    const role = roles.find((r) => r.id === roleId);
    const next: Record<string, number> = {};
    role?.permissions.forEach((p) => {
      next[p.permissionId] = p.scope;
    });
    setGrants(next);
    setEditRoleId(roleId);
  };

  const toggleGrant = (permissionId: string, maxScope: number) => {
    setGrants((current) => {
      if (permissionId in current) {
        const copy = { ...current };
        delete copy[permissionId];
        return copy;
      }
      return { ...current, [permissionId]: Math.min(PermissionScope.Organization, maxScope) };
    });
  };

  const submitCreate = async () => {
    try {
      await createRole({
        name,
        description: description || undefined,
        allowedClients: Number(allowedClients),
        grants: Object.entries(grants).map(([permissionId, scope]) => ({ permissionId, scope })),
      }).unwrap();
      toast.success(t('Tenancy.Roles.Created'));
      setCreateOpen(false);
    } catch (error) {
      showApiError(error, t);
    }
  };

  const submitEdit = async () => {
    if (!editRoleId) return;
    try {
      await updateRolePermissions({
        roleId: editRoleId,
        grants: Object.entries(grants).map(([permissionId, scope]) => ({ permissionId, scope })),
      }).unwrap();
      toast.success(t('Tenancy.Roles.Updated'));
      setEditRoleId(null);
    } catch (error) {
      showApiError(error, t);
    }
  };

  const grantEditor = (
    <div className="max-h-80 space-y-4 overflow-y-auto pr-1">
      {modules.map(([module, items]) => (
        <div key={module} className="space-y-2">
          <p className="text-sm font-semibold capitalize">{module}</p>
          {items.map((item) => (
            <div key={item.id} className="flex items-center justify-between gap-3">
              <label className="flex items-center gap-2 text-sm">
                <input
                  type="checkbox"
                  checked={item.id in grants}
                  onChange={() => toggleGrant(item.id, item.maxScope)}
                />
                {item.code}
              </label>
              {item.id in grants ? (
                <Select
                  value={String(grants[item.id])}
                  onValueChange={(value) =>
                    setGrants((current) => ({ ...current, [item.id]: Number(value) }))
                  }
                >
                  <SelectTrigger className="h-8 w-36">
                    <SelectValue />
                  </SelectTrigger>
                  <SelectContent>
                    {scopeLabels.slice(0, item.maxScope + 1).map((label, scope) => (
                      <SelectItem key={label} value={String(scope)}>
                        {label}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              ) : null}
            </div>
          ))}
        </div>
      ))}
    </div>
  );

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-bold">{t('Tenancy.Roles.Title')}</h1>
        <Can permission={Permissions.Tenancy.Roles.Manage}>
          <Button onClick={openCreate}>{t('Tenancy.Roles.New')}</Button>
        </Can>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>{t('Tenancy.Roles.Title')}</CardTitle>
        </CardHeader>
        <CardContent>
          {isLoading && <p className="text-muted-foreground">{t('Common.Loading')}</p>}
          {isError && (
            <div className="space-y-2">
              <p className="text-destructive">{t('Common.Error')}</p>
              <Button variant="outline" size="sm" onClick={() => void refetch()}>
                {t('Common.Retry')}
              </Button>
            </div>
          )}
          {!isLoading && !isError && (
            <table className="w-full text-sm">
              <thead>
                <tr className="border-b text-left">
                  <th className="py-2">{t('Tenancy.Roles.Name')}</th>
                  <th className="py-2">{t('Tenancy.Roles.System')}</th>
                  <th className="py-2">{t('Tenancy.Roles.Grants')}</th>
                  <th className="py-2">{t('Tenancy.Roles.Actions')}</th>
                </tr>
              </thead>
              <tbody>
                {roles.map((role) => (
                  <tr key={role.id} className="border-b">
                    <td className="py-2">{role.name}</td>
                    <td className="py-2">{role.isSystemRole ? t('Common.Yes') : t('Common.No')}</td>
                    <td className="py-2">{role.permissions.length}</td>
                    <td className="py-2">
                      <Can permission={Permissions.Tenancy.Roles.Manage}>
                        {!role.isSystemRole || canEditSystemRoles ? (
                          <Button variant="outline" size="sm" onClick={() => openEdit(role.id)}>
                            {t('Tenancy.Roles.EditPermissions')}
                          </Button>
                        ) : (
                          <span className="text-muted-foreground">{t('Tenancy.Roles.Protected')}</span>
                        )}
                      </Can>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
        </CardContent>
      </Card>

      <Dialog open={createOpen} onOpenChange={setCreateOpen}>
        <DialogContent className="max-w-2xl">
          <DialogHeader>
            <DialogTitle>{t('Tenancy.Roles.New')}</DialogTitle>
          </DialogHeader>
          <div className="space-y-3">
            <div className="space-y-1">
              <Label htmlFor="role-name">{t('Tenancy.Roles.Name')}</Label>
              <Input id="role-name" value={name} onChange={(e) => setName(e.target.value)} />
            </div>
            <div className="space-y-1">
              <Label htmlFor="role-description">{t('Tenancy.Roles.Description')}</Label>
              <Input id="role-description" value={description} onChange={(e) => setDescription(e.target.value)} />
            </div>
            <div className="space-y-1">
              <Label>{t('Tenancy.Roles.Clients')}</Label>
              <Select value={allowedClients} onValueChange={setAllowedClients}>
                <SelectTrigger>
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="1">Web</SelectItem>
                  <SelectItem value="2">Mobile</SelectItem>
                  <SelectItem value="3">All</SelectItem>
                </SelectContent>
              </Select>
            </div>
            {grantEditor}
          </div>
          <DialogFooter>
            <Button variant="outline" onClick={() => setCreateOpen(false)}>
              {t('Common.Cancel')}
            </Button>
            <Button onClick={() => void submitCreate()}>{t('Common.Save')}</Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      <Dialog open={editRoleId !== null} onOpenChange={(next) => !next && setEditRoleId(null)}>
        <DialogContent className="max-w-2xl">
          <DialogHeader>
            <DialogTitle>
              {t('Tenancy.Roles.EditPermissions')} {editing ? `— ${editing.name}` : ''}
            </DialogTitle>
          </DialogHeader>
          {grantEditor}
          <DialogFooter>
            <Button variant="outline" onClick={() => setEditRoleId(null)}>
              {t('Common.Cancel')}
            </Button>
            <Button onClick={() => void submitEdit()}>{t('Common.Save')}</Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
}
