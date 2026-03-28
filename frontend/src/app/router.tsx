import type { ReactNode } from 'react'
import { Navigate, Route, Routes } from 'react-router-dom'
import { ProtectedLayout } from '@/app/layouts/ProtectedLayout'
import { useAuthStore } from '@/features/auth/model/auth.store'
import { GamePage } from '@/pages/GamePage'
import { LoginPage } from '@/pages/LoginPage'
import { ProfilePage } from '@/pages/ProfilePage'
import { RegisterPage } from '@/pages/RegisterPage'

const ProtectedRoute = () => {
  const { token } = useAuthStore()
  return token ? <ProtectedLayout /> : <Navigate to="/login" replace />
}

const PublicOnlyRoute = ({ children }: { children: ReactNode }) => {
  const { token } = useAuthStore()
  return token ? <Navigate to="/game" replace /> : children
}

const RootRedirect = () => {
  const { token } = useAuthStore()
  return <Navigate to={token ? '/game' : '/login'} replace />
}

export const AppRouter = () => {
  return (
    <Routes>
      <Route path="/" element={<RootRedirect />} />
      <Route
        path="/login"
        element={
          <PublicOnlyRoute>
            <LoginPage />
          </PublicOnlyRoute>
        }
      />
      <Route
        path="/register"
        element={
          <PublicOnlyRoute>
            <RegisterPage />
          </PublicOnlyRoute>
        }
      />
      <Route
        element={<ProtectedRoute />}
      >
        <Route path="/game" element={<GamePage />} />
        <Route path="/profile" element={<ProfilePage />} />
      </Route>
      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  )
}
