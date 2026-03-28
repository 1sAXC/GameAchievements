import { useState } from 'react'
import axios from 'axios'
import { z } from 'zod'
import { Link, useNavigate } from 'react-router-dom'
import { authApi } from '@/api/auth.api'

const registerSchema = z.object({
  email: z.string().email('Введите корректный email'),
  password: z
    .string()
    .min(6, 'Пароль должен содержать не менее 6 символов')
    .max(128, 'Слишком длинный пароль'),
})

export const RegisterPage = () => {
  const navigate = useNavigate()
  const [email, setEmail] = useState('player@example.com')
  const [password, setPassword] = useState('P@ssw0rd123')
  const [emailError, setEmailError] = useState<string | null>(null)
  const [passwordError, setPasswordError] = useState<string | null>(null)
  const [requestError, setRequestError] = useState<string | null>(null)
  const [successMessage, setSuccessMessage] = useState<string | null>(null)
  const [isLoading, setIsLoading] = useState(false)

  const onSubmit = async (event: React.FormEvent) => {
    event.preventDefault()
    setEmailError(null)
    setPasswordError(null)
    setRequestError(null)
    setSuccessMessage(null)

    const validation = registerSchema.safeParse({
      email: email.trim(),
      password,
    })

    if (!validation.success) {
      const errors = validation.error.flatten().fieldErrors
      setEmailError(errors.email?.[0] ?? null)
      setPasswordError(errors.password?.[0] ?? null)
      return
    }

    setIsLoading(true)

    try {
      await authApi.register(validation.data)
      setSuccessMessage('Регистрация успешна. Переходим ко входу...')
      window.setTimeout(() => {
        navigate('/login', { replace: true })
      }, 900)
    } catch (submitError) {
      if (axios.isAxiosError(submitError)) {
        const status = submitError.response?.status
        if (status === 400) {
          setRequestError('Ошибка валидации: email и пароль не должны быть пустыми.')
        } else if (status === 409) {
          setRequestError('Пользователь с таким email уже существует.')
        } else {
          setRequestError(`Не удалось зарегистрироваться (${status ?? 'ошибка сети'}).`)
        }
      } else {
        setRequestError('Не удалось зарегистрироваться.')
      }
    } finally {
      setIsLoading(false)
    }
  }

  return (
    <main className="auth-page">
      <section className="auth-card">
        <p className="auth-kicker">Игровые достижения</p>
        <h1>Регистрация</h1>
        <p className="hint">Создайте аккаунт, чтобы отслеживать матчи и достижения.</p>

        <form className="form" onSubmit={onSubmit} noValidate>
          <label htmlFor="register-email">Эл. почта</label>
          <input
            id="register-email"
            type="email"
            autoComplete="email"
            value={email}
            onChange={(event) => setEmail(event.target.value)}
            required
            aria-invalid={emailError ? 'true' : 'false'}
          />
          {emailError ? <p className="error">{emailError}</p> : null}

          <label htmlFor="register-password">Пароль</label>
          <input
            id="register-password"
            type="password"
            autoComplete="new-password"
            value={password}
            onChange={(event) => setPassword(event.target.value)}
            required
            aria-invalid={passwordError ? 'true' : 'false'}
          />
          {passwordError ? <p className="error">{passwordError}</p> : null}

          <button type="submit" disabled={isLoading}>
            {isLoading ? 'Создаём...' : 'Зарегистрироваться'}
          </button>
        </form>

        {requestError ? <p className="error">{requestError}</p> : null}
        {successMessage ? <p className="success">{successMessage}</p> : null}

        <p className="hint">
          Уже есть аккаунт? <Link to="/login">Войти</Link>
        </p>
      </section>
    </main>
  )
}
