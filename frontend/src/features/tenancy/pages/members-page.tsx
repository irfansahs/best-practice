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
import { Permissions } from '@/shared/api/api-types';
import { Can } from '@/shared/components/can';
import { showApiError } from '@/shared/api/show-api-error';
import { useAppSelector } from '@/app/hooks';
import { selectActiveOrganization } from '@/features/auth/slice/auth-slice';
import {
  useAddMemberMutation,
  useChangeMemberStatusMutation,
  useGetMembersQuery,
  useGetOrganizationsQuery,
  useGetRolesQuery,
} from '@/features/tenancy/api/tenancy-api';

export function MembersPage() {
  const { t } = useTranslation();
  const active = useAppSelector(selectActiveOrganization);
  const { data: orgsData } = useGetOrganizationsQuery();
  const organizations = orgsData?.data ?? [];
  const [organizationId, setOrganizationId] = useState(active?.id ?? '');
  const selectedOrg = organizationId || active?.id || organizations[0]?.id || '';
  const { data, isLoading, isError, refetch } = useGetMembersQuery(selectedOrg, { skip: !selectedOrg });
  const { data: rolesData } = useGetRolesQuery();
  const [addMember] = useAddMemberMutation();
  const [changeStatus] = useChangeMemberStatusMutation();
  const [open, setOpen] = useState(false);
  const [userId, setUserId] = useState('');
  const [title, setTitle] = useState('');
  const [roleId, setRoleId] = useState('');

  const members = data?.data ?? [];
  const roles = rolesData?.data ?? [];
  const defaultRoleId = useMemo(() => roles[0]?.id ?? '', [roles]);

  const submit = async () => {
    if (!selectedOrg) return;
    try {
      await addMember({
        organizationId: selectedOrg,
        userId,
        roleIds: [roleId || defaultRoleId].filter(Boolean),
        title: title || undefined,
        isPrimary: false,
      }).unwrap();
      toast.success(t('Tenancy.Members.Added'));
      setOpen(false);
      setUserId('');
      setTitle('');
    } catch (error) {
      showApiError(error, t);
    }
  };

  const onStatus = async (membershipId: string, status: string) => {
    try {
      await changeStatus({ membershipId, organizationId: selectedOrg, status }).unwrap();
      toast.success(t('Tenancy.Members.StatusUpdated'));
    } catch (error) {
      showApiError(error, t);
    }
  };

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-bold">{t('Tenancy.Members.Title')}</h1>
        <Can permission={Permissions.Tenancy.Members.Manage}>
          <Button onClick={() => setOpen(true)} disabled={!selectedOrg}>
            {t('Tenancy.Members.Add')}
          </Button>
        </Can>
      </div>

      <div className="max-w-sm space-y-1">
        <Label>{t('Tenancy.Members.Organization')}</Label>
        <Select value={selectedOrg} onValueChange={setOrganizationId}>
          <SelectTrigger>
            <SelectValue placeholder={t('Tenancy.Members.SelectOrganization')} />
          </SelectTrigger>
          <SelectContent>
            {organizations.map((org) => (
              <SelectItem key={org.id} value={org.id}>
                {org.name}
              </SelectItem>
            ))}
          </SelectContent>
        </Select>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>{t('Tenancy.Members.Title')}</CardTitle>
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
          {!isLoading && !isError && members.length === 0 && (
            <p className="text-muted-foreground">{t('Tenancy.Members.Empty')}</p>
          )}
          {!isLoading && !isError && members.length > 0 && (
            <table className="w-full text-sm">
              <thead>
                <tr className="border-b text-left">
                  <th className="py-2">{t('Tenancy.Members.Name')}</th>
                  <th className="py-2">{t('Tenancy.Members.Email')}</th>
                  <th className="py-2">{t('Tenancy.Members.Roles')}</th>
                  <th className="py-2">{t('Tenancy.Members.Status')}</th>
                  <th className="py-2">{t('Tenancy.Members.Actions')}</th>
                </tr>
              </thead>
              <tbody>
                {members.map((member) => (
                  <tr key={member.membershipId} className="border-b">
                    <td className="py-2">{member.fullName}</td>
                    <td className="py-2">{member.email}</td>
                    <td className="py-2">{member.roles.join(', ')}</td>
                    <td className="py-2">{member.status}</td>
                    <td className="py-2">
                      <Can permission={Permissions.Tenancy.Members.Manage}>
                        {member.status !== 'Suspended' ? (
                          <Button
                            variant="outline"
                            size="sm"
                            onClick={() => void onStatus(member.membershipId, 'Suspended')}
                          >
                            {t('Tenancy.Members.Suspend')}
                          </Button>
                        ) : (
                          <Button
                            variant="outline"
                            size="sm"
                            onClick={() => void onStatus(member.membershipId, 'Active')}
                          >
                            {t('Tenancy.Members.Activate')}
                          </Button>
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

      <Dialog open={open} onOpenChange={setOpen}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>{t('Tenancy.Members.Add')}</DialogTitle>
          </DialogHeader>
          <div className="space-y-3">
            <div className="space-y-1">
              <Label htmlFor="member-user">{t('Tenancy.Members.UserId')}</Label>
              <Input id="member-user" value={userId} onChange={(e) => setUserId(e.target.value)} />
            </div>
            <div className="space-y-1">
              <Label htmlFor="member-title">{t('Tenancy.Members.TitleLabel')}</Label>
              <Input id="member-title" value={title} onChange={(e) => setTitle(e.target.value)} />
            </div>
            <div className="space-y-1">
              <Label>{t('Tenancy.Members.Role')}</Label>
              <Select value={roleId || defaultRoleId} onValueChange={setRoleId}>
                <SelectTrigger>
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  {roles.map((role) => (
                    <SelectItem key={role.id} value={role.id}>
                      {role.name}
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
