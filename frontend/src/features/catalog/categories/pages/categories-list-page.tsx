import { Link } from 'react-router';
import { useTranslation } from 'react-i18next';
import { Plus } from 'lucide-react';
import { toast } from 'sonner';
import { Button } from '@/shared/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/shared/components/ui/card';
import { Permissions } from '@/shared/api/api-types';
import { PermissionGate } from '@/app/routes/permission-gate';
import {
  useDeleteCategoryMutation,
  useGetCategoriesQuery,
} from '@/features/catalog/categories/api/categories-api';
import { showApiError } from '@/shared/api/show-api-error';

export function CategoriesListPage() {
  const { t } = useTranslation();
  const { data, isLoading, isError, refetch } = useGetCategoriesQuery();
  const [deleteCategory] = useDeleteCategoryMutation();

  const categories = data?.data ?? [];

  const handleDelete = async (id: string, name: string) => {
    if (!window.confirm(t('Categories.Delete.Confirm', { name }))) return;
    try {
      await deleteCategory(id).unwrap();
      toast.success(t('Categories.Delete.Success'));
    } catch (error) {
      showApiError(error, t);
    }
  };

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-bold">{t('Categories.Title')}</h1>
        <PermissionGate permission={Permissions.Catalog.Categories.Create} fallback={null}>
          <Button asChild>
            <Link to="/categories/new">
              <Plus className="mr-2 h-4 w-4" />
              {t('Categories.New')}
            </Link>
          </Button>
        </PermissionGate>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>{t('Categories.Title')}</CardTitle>
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
          {!isLoading && !isError && categories.length === 0 && (
            <p className="text-muted-foreground">{t('Categories.Empty')}</p>
          )}
          {!isLoading && !isError && categories.length > 0 && (
            <div className="overflow-x-auto">
              <table className="w-full text-sm">
                <thead>
                  <tr className="border-b text-left">
                    <th className="py-2">{t('Categories.Table.Name')}</th>
                    <th className="py-2">{t('Categories.Table.Status')}</th>
                    <th className="py-2">{t('Categories.Table.Actions')}</th>
                  </tr>
                </thead>
                <tbody>
                  {categories.map((category) => (
                    <tr key={category.id} className="border-b">
                      <td className="py-2">{category.name}</td>
                      <td className="py-2">
                        {category.isActive ? t('Products.Status.Active') : t('Products.Status.Inactive')}
                      </td>
                      <td className="py-2">
                        <div className="flex gap-2">
                          <PermissionGate permission={Permissions.Catalog.Categories.Update} fallback={null}>
                            <Button variant="outline" size="sm" asChild>
                              <Link to={`/categories/${category.id}/edit`}>{t('Common.Edit')}</Link>
                            </Button>
                          </PermissionGate>
                          <PermissionGate permission={Permissions.Catalog.Categories.Delete} fallback={null}>
                            <Button
                              variant="destructive"
                              size="sm"
                              onClick={() => void handleDelete(category.id, category.name)}
                            >
                              {t('Common.Delete')}
                            </Button>
                          </PermissionGate>
                        </div>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  );
}
