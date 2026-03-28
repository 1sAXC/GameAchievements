import { createContext, useContext, useMemo, useReducer } from 'react'
import type { ReactNode } from 'react'
import type { AuthUserResponse } from '@/api/dto'
import { storageKeys } from '@/shared/config/storageKeys'

interface AuthState {
  token: string | null
  user: AuthUserResponse | null
}

interface AuthStore extends AuthState {
  setToken: (token: string | null) => void
  setUser: (user: AuthUserResponse | null) => void
  logout: () => void
}

type AuthAction =
  | { type: 'set_token'; payload: string | null }
  | { type: 'set_user'; payload: AuthUserResponse | null }
  | { type: 'logout' }

const authReducer = (state: AuthState, action: AuthAction): AuthState => {
  switch (action.type) {
    case 'set_token':
      return { ...state, token: action.payload }
    case 'set_user':
      return { ...state, user: action.payload }
    case 'logout':
      return { token: null, user: null }
    default:
      return state
  }
}

const readStoredUser = (): AuthUserResponse | null => {
  const rawValue = localStorage.getItem(storageKeys.authUser)
  if (!rawValue) {
    return null
  }

  try {
    return JSON.parse(rawValue) as AuthUserResponse
  } catch {
    localStorage.removeItem(storageKeys.authUser)
    return null
  }
}

const createInitialState = (): AuthState => ({
  token: localStorage.getItem(storageKeys.accessToken),
  user: readStoredUser(),
})

const AuthStoreContext = createContext<AuthStore | null>(null)

export const AuthProvider = ({ children }: { children: ReactNode }) => {
  const [state, dispatch] = useReducer(authReducer, undefined, createInitialState)

  const setToken = (token: string | null) => {
    if (token) {
      localStorage.setItem(storageKeys.accessToken, token)
    } else {
      localStorage.removeItem(storageKeys.accessToken)
    }

    dispatch({ type: 'set_token', payload: token })
  }

  const setUser = (user: AuthUserResponse | null) => {
    if (user) {
      localStorage.setItem(storageKeys.authUser, JSON.stringify(user))
    } else {
      localStorage.removeItem(storageKeys.authUser)
    }

    dispatch({ type: 'set_user', payload: user })
  }

  const logout = () => {
    localStorage.removeItem(storageKeys.accessToken)
    localStorage.removeItem(storageKeys.authUser)
    dispatch({ type: 'logout' })
  }

  const value = useMemo(
    () => ({
      token: state.token,
      user: state.user,
      setToken,
      setUser,
      logout,
    }),
    [state.token, state.user],
  )

  return <AuthStoreContext.Provider value={value}>{children}</AuthStoreContext.Provider>
}

export const useAuthStore = () => {
  const context = useContext(AuthStoreContext)
  if (!context) {
    throw new Error('useAuthStore должен использоваться внутри AuthProvider')
  }

  return context
}
