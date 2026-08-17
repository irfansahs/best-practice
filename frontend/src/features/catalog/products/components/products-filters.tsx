import { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Search } from 'lucide-react';
import { Input } from '@/shared/components/ui/input';
import { useAppDispatch, useAppSelector } from '@/app/hooks';
import { selectProductsSearch, setSearch } from '@/features/catalog/products/slice/products-slice';

export function ProductsFilters() {
  const { t } = useTranslation();
  const dispatch = useAppDispatch();
  const search = useAppSelector(selectProductsSearch);
  const [localSearch, setLocalSearch] = useState(search);

  useEffect(() => {
    const timer = window.setTimeout(() => {
      dispatch(setSearch(localSearch));
    }, 300);

    return () => window.clearTimeout(timer);
  }, [dispatch, localSearch]);

  return (
    <div className="relative max-w-sm">
      <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
      <Input
        className="pl-9"
        placeholder={t('Products.Search')}
        value={localSearch}
        onChange={(event) => setLocalSearch(event.target.value)}
      />
    </div>
  );
}
