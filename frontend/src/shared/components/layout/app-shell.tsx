import { Outlet } from 'react-router';
import { Sidebar } from '@/shared/components/layout/sidebar';
import { Topbar } from '@/shared/components/layout/topbar';

export function AppShell() {
  return (
    <div className="flex min-h-screen">
      <Sidebar />
      <div className="flex flex-1 flex-col">
        <Topbar />
        <main className="flex-1 overflow-auto p-6">
          <Outlet />
        </main>
      </div>
    </div>
  );
}
