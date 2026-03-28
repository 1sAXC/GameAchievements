import { achievementsHttp } from '@/api/http'
import type { AchievementDefinitionResponse, AchievementDto } from '@/api/dto'

export const achievementsApi = {
  getDefinitions: async () => {
    const { data } = await achievementsHttp.get<AchievementDefinitionResponse[]>('/achievements')
    return data
  },
  getMyAchievements: async () => {
    const { data } = await achievementsHttp.get<AchievementDto[]>('/users/me/achievements')
    return data
  },
}
