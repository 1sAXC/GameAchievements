const requiredEnv = (name: 'VITE_AUTH_API' | 'VITE_RESULTS_API' | 'VITE_ACH_API') => {
  const value = import.meta.env[name]

  if (!value) {
    throw new Error(`Отсутствует переменная окружения: ${name}`)
  }

  return value
}

export const env = {
  authApi: requiredEnv('VITE_AUTH_API'),
  resultsApi: requiredEnv('VITE_RESULTS_API'),
  achievementsApi: requiredEnv('VITE_ACH_API'),
} as const
