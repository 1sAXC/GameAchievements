import { NavLink, useNavigate } from 'react-router-dom'
import { useAuthStore } from '@/features/auth/model/auth.store'

export const AppHeader = () => {
  const navigate = useNavigate()
  const { user, logout } = useAuthStore()

  const onLogout = () => {
    logout()
    navigate('/login', { replace: true })
  }

  return (
    <header className="app-header">
      <div className="header-brand">
        <p className="brand-title">Игровые достижения</p>
        <p className="brand-subtitle">{user?.email ?? 'Неизвестный пилот'}</p>
      </div>
      <nav className="header-nav" aria-label="Основная навигация">
        <NavLink to="/game" className="nav-link">
          Игра
        </NavLink>
        <NavLink to="/profile" className="nav-link">
          Профиль
        </NavLink>
      </nav>
      <button type="button" className="logout-btn" onClick={onLogout}>
        Выйти
      </button>
    </header>
  )
}
