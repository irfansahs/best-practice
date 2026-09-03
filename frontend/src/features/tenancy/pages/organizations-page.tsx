import { useState } from 'react';
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
import { Permissions } from '@/shared/api/api-types';
import { Can } from '@/shared/components/can';
import { showApiError } from '@/shared/api/show-api-error';
import { useAppDispatch } from '@/app/hooks';
import { switchOrganization } from '@/features/auth/slice/auth-slice';
import {
  useChangeOrganizationStatusMutation,
  useCreateOrganizationMutation,
  useGetOrganizationsQuery,
} from '@/features/tenancy/api/tenancy-api';

export function OrganizationsPage() {
  const { t } = useTranslation();
  const dispatch = useAppDispatch();
  const { data, isLoading, isError, refetch } = useGetOrganizationsQuery();
  const [createOrganization] = useCreateOrganizationMutation();
  const [changeStatus] = useChangeOrganizationStatusMutation();
  const [open, setOpen] = useState(false);
  const [name, setName] = useState('');
  const [slug, setSlug] = useState('');
  const [parentId, setParentId] = useState<string>('none');

  const organizations = data?.data ?? [];

  const submit = async () => {
    try {
      await createOrganization({
        name,
        slug,
        parentId: parentId === 'none' ? undefined : parentId,
      }).unwrap();
      toast.success(t('Tenancy.Organizations.Created'));
      setOpen(false);
      setName('');
      setSlug('');
      setParentId('none');
    } catch (error) {
      showApiError(error, t);
    }
  };

  const onStatus = async (id: string, status: string) => {
    try {
      await changeStatus({ id, status }).unwrap();
      toast.success(t('Tenancy.Organizations.StatusUpdated'));
    } catch (error) {
      showApiError(error, t);
    }
  };

  const onSwitch = async (id: string) => {
    try {
      await dispatch(switchOrganization(id)).unwrap();
      toast.success(t('Auth.Switch.Success'));
    } catch (error) {
      if (typeof error === 'string') toast.error(t(error));
      else showApiError(error, t);
    }
  };

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-bold">{t('Tenancy.Organizations.Title')}</h1>
        <Can permission={Permissions.Tenancy.Organizations.Create}>
          <Button onClick={() => setOpen(true)}>{t('Tenancy.Organizations.New')}</Button>
        </Can>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>{t('Tenancy.Organizations.Title')}</CardTitle>
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
          {!isLoading && !isError && organizations.length === 0 && (
            <p className="text-muted-foreground">{t('Tenancy.Organizations.Empty')}</p>
          )}
          {!isLoading && !isError && organizations.length > 0 && (
            <table className="w-full text-sm">
              <thead>
                <tr className="border-b text-left">
                  <th className="py-2">{t('Tenancy.Organizations.Name')}</th>
                  <th className="py-2">{t('Tenancy.Organizations.Type')}</th>
                  <th className="py-2">{t('Tenancy.Organizations.Status')}</th>
                  <th className="py-2">{t('Tenancy.Organizations.Actions')}</th>
                </tr>
              </thead>
              <tbody>
                {organizations.map((org) => (
                  <tr key={org.id} className="border-b">
                    <td className="py-2" style={{ paddingLeft: `${org.depth * 16}px` }}>
                      {org.name}
                    </td>
                    <td className="py-2">{org.type}</td>
                    <td className="py-2">{org.status}</td>
                    <td className="py-2">
                      <div className="flex flex-wrap gap-2">
                        <Button variant="outline" size="sm" onClick={() => void onSwitch(org.id)}>
                          {t('Auth.Switch.Action')}
                        </Button>
                        <Can permission={Permissions.Tenancy.Organizations.Update}>
                          {org.status !== 'Suspended' ? (
                            <Button variant="outline" size="sm" onClick={() => void onStatus(org.id, 'Suspended')}>
                              {t('Tenancy.Organizations.Suspend')}
                            </Button>
                          ) : (
                            <Button variant="outline" size="sm" onClick={() => void onStatus(org.id, 'Active')}>
                              {t('Tenancy.Organizations.Activate')}
                            </Button>
                          )}
                        </Can>
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
        </CardContent>
      </Card>

      <Dialog open={open} onOpenChange={setOpen}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>{t('Tenancy.Organizations.New')}</DialogTitle>
          </DialogHeader>
          <div className="space-y-3">
            <div className="space-y-1">
              <Label htmlFor="org-name">{t('Tenancy.Organizations.Name')}</Label>
              <Input id="org-name" value={name} onChange={(e) => setName(e.target.value)} />
            </div>
            <div className="space-y-1">
              <Label htmlFor="org-slug">{t('Tenancy.Organizations.Slug')}</Label>
              <Input id="org-slug" value={slug} onChange={(e) => setSlug(e.target.value)} />
            </div>
            <div className="space-y-1">
              <Label>{t('Tenancy.Organizations.Parent')}</Label>
              <Select value={parentId} onValueChange={setParentId}>
                <SelectTrigger>
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="none">{t('Tenancy.Organizations.CurrentParent')}</SelectItem>
                  {organizations.map((org) => (
                    <SelectItem key={org.id} value={org.id}>
                      {org.name}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
          </div>
          <DialogFooter>
            <Button variant="outline" onClick={() => setOpen(false)}>
              {t('Common.Cancel')}
            </Button>
            <Button onClick={() => void submit()}>{t('Common.Save')}</Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
}
