export interface RegisterRequest {
  email: string
  password: string
}

export interface LoginRequest {
  email: string
  password: string
}

export interface AuthTokenResponse {
  accessToken: string
  expiresAtUtc: string
}

export interface AuthUserResponse {
  id: string
  email: string
  createdAtUtc: string
}

export interface CreateResultRequest {
  gameId: string
  score: number
  kills: number
  deaths: number
  win: boolean
  playedAt: string
}

export interface ResultResponse {
  id: string
  userId: string
  gameId: string
  score: number
  kills: number
  deaths: number
  isWin: boolean
  isPerfect: boolean
  playedAt: string
  createdAtUtc: string
}

export interface ResultRecordedResponse {
  resultId: string
  userId: string
  gameId: string
  score: number
  kills: number
  deaths: number
  win: boolean
  isPerfect: boolean
  playedAt: string
}

export interface AchievementDefinitionResponse {
  code: string
  name: string
  description: string
  type: string
  targetValue: number | null
}

export interface AchievementDto {
  code: string
  name: string
  description: string
  grantedAt: string
}
