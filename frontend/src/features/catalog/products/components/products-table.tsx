import { useState } from 'react';
import { Link } from 'react-router';
import { useTranslation } from 'react-i18next';
import { Pencil, Trash2 } from 'lucide-react';
import { toast } from 'sonner';
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/shared/components/ui/table';
import { Button } from '@/shared/components/ui/button';
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from '@/shared/components/ui/alert-dialog';
import type { ProductListItem } from '@/shared/api/api-types';
import { Permissions } from '@/shared/api/api-types';
import { usePermission } from '@/features/auth/hooks/use-permission';
import { useDeleteProductMutation } from '@/features/catalog/products/api/products-api';
import { showApiError } from '@/shared/api/show-api-error';

interface ProductsTableProps {
  items: ProductListItem[];
  isLoading?: boolean;
}

export function ProductsTable({ items, isLoading }: ProductsTableProps) {
  const { t } = useTranslation();
  const [deleteProduct] = useDeleteProductMutation();
  const canUpdate = usePermission(Permissions.Catalog.Products.Update);
  const canDelete = usePermission(Permissions.Catalog.Products.Delete);
  const [pendingDeleteId, setPendingDeleteId] = useState<string | null>(null);

  const handleDelete = async () => {
    if (!pendingDeleteId) return;
    try {
      await deleteProduct(pendingDeleteId).unwrap();
      toast.success(t('Products.Delete.Success'));
      setPendingDeleteId(null);
    } catch (error) {
      showApiError(error, t);
    }
  };

  if (isLoading) {
    return <p className="text-muted-foreground">{t('Common.Loading')}</p>;
  }

  if (items.length === 0) {
    return <p className="text-muted-foreground">{t('Products.Empty')}</p>;
  }

  return (
    <>
      <Table>
        <TableHeader>
          <TableRow>
            <TableHead>{t('Products.Table.Sku')}</TableHead>
            <TableHead>{t('Products.Table.Name')}</TableHead>
            <TableHead>{t('Products.Table.Price')}</TableHead>
            <TableHead>{t('Products.Table.Status')}</TableHead>
            {(canUpdate || canDelete) && <TableHead className="text-right">{t('Products.Table.Actions')}</TableHead>}
          </TableRow>
        </TableHeader>
        <TableBody>
          {items.map((product) => (
            <TableRow key={product.id}>
              <TableCell className="font-mono text-sm">{product.sku}</TableCell>
              <TableCell>{product.name}</TableCell>
              <TableCell>
                {product.price.toFixed(2)} {product.currency}
              </TableCell>
              <TableCell>
                {product.isActive ? t('Products.Status.Active') : t('Products.Status.Inactive')}
              </TableCell>
              {(canUpdate || canDelete) && (
                <TableCell className="text-right">
                  <div className="flex justify-end gap-2">
                    {canUpdate && (
                      <Button variant="outline" size="icon" asChild>
                        <Link to={`/products/${product.id}/edit`}>
                          <Pencil className="h-4 w-4" />
                        </Link>
                      </Button>
                    )}
                    {canDelete && (
                      <Button variant="destructive" size="icon" onClick={() => setPendingDeleteId(product.id)}>
                        <Trash2 className="h-4 w-4" />
                      </Button>
                    )}
                  </div>
                </TableCell>
              )}
            </TableRow>
          ))}
        </TableBody>
      </Table>

      <AlertDialog open={pendingDeleteId !== null} onOpenChange={(open) => !open && setPendingDeleteId(null)}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>{t('Products.Delete.Confirm')}</AlertDialogTitle>
            <AlertDialogDescription>{t('Products.Delete.Confirm')}</AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>{t('Common.Cancel')}</AlertDialogCancel>
            <AlertDialogAction onClick={() => void handleDelete()}>{t('Common.Delete')}</AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </>
  );
}
