import axios from 'axios'
import type { AxiosError, AxiosInstance } from 'axios'
import { env } from '@/shared/config/env'
import { storageKeys } from '@/shared/config/storageKeys'

const getAccessToken = () => localStorage.getItem(storageKeys.accessToken)
const clearSession = () => {
  localStorage.removeItem(storageKeys.accessToken)
  localStorage.removeItem(storageKeys.authUser)
}

const shouldSkipAuthRedirect = (requestUrl?: string) => {
  const url = requestUrl ?? ''
  return url.includes('/auth/login') || url.includes('/auth/register')
}

const redirectToLogin = () => {
  const { pathname } = window.location
  if (pathname !== '/login' && pathname !== '/register') {
    window.location.assign('/login')
  }
}

const createHttpClient = (baseURL: string): AxiosInstance => {
  const client = axios.create({
    baseURL,
    timeout: 10000,
    headers: {
      Accept: 'application/json',
      'Content-Type': 'application/json',
    },
  })

  client.interceptors.request.use((config) => {
    const token = getAccessToken()
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }

    return config
  })

  client.interceptors.response.use(
    (response) => response,
    (error: AxiosError) => {
      if (error.response?.status === 401 && !shouldSkipAuthRedirect(error.config?.url)) {
        clearSession()
        redirectToLogin()
      }

      return Promise.reject(error)
    },
  )

  return client
}

export const authHttp = createHttpClient(env.authApi)
export const resultsHttp = createHttpClient(env.resultsApi)
export const achievementsHttp = createHttpClient(env.achievementsApi)
