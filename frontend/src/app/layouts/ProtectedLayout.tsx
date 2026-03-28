import { Outlet } from 'react-router-dom'
import { AppHeader } from '@/app/layouts/AppHeader'

export const ProtectedLayout = () => {
  return (
    <div className="app-shell">
      <AppHeader />
      <main className="app-content">
        <Outlet />
      </main>
    </div>
  )
}
