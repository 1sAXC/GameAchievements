import { useState } from 'react'
import axios from 'axios'
import { Link, useNavigate } from 'react-router-dom'
import { authApi } from '@/api/auth.api'
import { useAuthStore } from '@/features/auth/model/auth.store'

export const LoginPage = () => {
  const navigate = useNavigate()
  const { setToken, setUser } = useAuthStore()
  const [email, setEmail] = useState('player@example.com')
  const [password, setPassword] = useState('P@ssw0rd123')
  const [error, setError] = useState<string | null>(null)
  const [isLoading, setIsLoading] = useState(false)

  const onSubmit = async (event: React.FormEvent) => {
    event.preventDefault()
    setError(null)
    setIsLoading(true)

    try {
      const tokenResponse = await authApi.login({ email: email.trim(), password })
      setToken(tokenResponse.accessToken)

      const user = await authApi.me()
      setUser(user)
      navigate('/game', { replace: true })
    } catch (submitError) {
      if (axios.isAxiosError(submitError) && submitError.response?.status === 401) {
        setError('Неверный email или пароль')
      } else {
        setError('Сейчас не удалось выполнить вход. Попробуйте снова.')
      }

      setToken(null)
      setUser(null)
    } finally {
      setIsLoading(false)
    }
  }

  return (
    <main className="auth-page">
      <section className="auth-card">
        <p className="auth-kicker">Игровые достижения</p>
        <h1>Вход</h1>
        <p className="hint">Войдите, чтобы открыть профиль и ленту достижений.</p>

        <form className="form" onSubmit={onSubmit} noValidate>
          <label htmlFor="login-email">Эл. почта</label>
          <input
            id="login-email"
            type="email"
            autoComplete="email"
            value={email}
            onChange={(event) => setEmail(event.target.value)}
            required
            aria-invalid={error ? 'true' : 'false'}
          />

          <label htmlFor="login-password">Пароль</label>
          <input
            id="login-password"
            type="password"
            autoComplete="current-password"
            value={password}
            onChange={(event) => setPassword(event.target.value)}
            required
            aria-invalid={error ? 'true' : 'false'}
          />

          <button type="submit" disabled={isLoading}>
            {isLoading ? 'Входим...' : 'Войти'}
          </button>
        </form>

        {error ? <p className="error">{error}</p> : null}

        <p className="hint">
          Нет аккаунта? <Link to="/register">Зарегистрироваться</Link>
        </p>
      </section>
    </main>
  )
}
