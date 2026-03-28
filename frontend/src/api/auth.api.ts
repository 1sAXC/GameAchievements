import { authHttp } from '@/api/http'
import type {
  AuthTokenResponse,
  AuthUserResponse,
  LoginRequest,
  RegisterRequest,
} from '@/api/dto'

export const authApi = {
  register: async (request: RegisterRequest) => {
    const { data } = await authHttp.post<AuthUserResponse>('/auth/register', request)
    return data
  },
  login: async (request: LoginRequest) => {
    const { data } = await authHttp.post<AuthTokenResponse>('/auth/login', request)
    return data
  },
  me: async () => {
    const { data } = await authHttp.get<AuthUserResponse>('/auth/me')
    return data
  },
}
