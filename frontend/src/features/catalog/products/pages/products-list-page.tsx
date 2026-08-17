import { Link } from 'react-router';
import { useTranslation } from 'react-i18next';
import { Plus } from 'lucide-react';
import { Button } from '@/shared/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/shared/components/ui/card';
import { Permissions } from '@/shared/api/api-types';
import { useAppDispatch, useAppSelector } from '@/app/hooks';
import {
  selectProductsPage,
  selectProductsPageSize,
  selectProductsSearch,
  setPage,
} from '@/features/catalog/products/slice/products-slice';
import { useGetProductsQuery } from '@/features/catalog/products/api/products-api';
import { ProductsFilters } from '@/features/catalog/products/components/products-filters';
import { ProductsTable } from '@/features/catalog/products/components/products-table';
import { PermissionGate } from '@/app/routes/permission-gate';

export function ProductsListPage() {
  const { t } = useTranslation();
  const dispatch = useAppDispatch();
  const page = useAppSelector(selectProductsPage);
  const pageSize = useAppSelector(selectProductsPageSize);
  const search = useAppSelector(selectProductsSearch);

  const { data, isLoading, isFetching, isError, refetch } = useGetProductsQuery({ page, pageSize, search });

  const paged = data?.data;
  const totalPages = paged?.totalPages ?? 1;

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-bold">{t('Products.Title')}</h1>
        <PermissionGate permission={Permissions.Catalog.Products.Create} fallback={null}>
          <Button asChild>
            <Link to="/products/new">
              <Plus className="mr-2 h-4 w-4" />
              {t('Products.New')}
            </Link>
          </Button>
        </PermissionGate>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>{t('Products.Title')}</CardTitle>
        </CardHeader>
        <CardContent className="space-y-4">
          <ProductsFilters />
          {isError ? (
            <div className="space-y-2">
              <p className="text-destructive">{t('Common.Error')}</p>
              <Button variant="outline" size="sm" onClick={() => void refetch()}>
                {t('Common.Retry')}
              </Button>
            </div>
          ) : (
            <ProductsTable items={paged?.items ?? []} isLoading={isLoading || isFetching} />
          )}

          {paged && totalPages > 1 && !isError && (
            <div className="flex items-center justify-between pt-4">
              <p className="text-sm text-muted-foreground">
                {t('Common.Page', { page: paged.page, total: totalPages })}
              </p>
              <div className="flex gap-2">
                <Button
                  variant="outline"
                  size="sm"
                  disabled={page <= 1}
                  onClick={() => dispatch(setPage(page - 1))}
                >
                  {t('Common.Previous')}
                </Button>
                <Button
                  variant="outline"
                  size="sm"
                  disabled={!paged.hasNext}
                  onClick={() => dispatch(setPage(page + 1))}
                >
                  {t('Common.Next')}
                </Button>
              </div>
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  );
}
