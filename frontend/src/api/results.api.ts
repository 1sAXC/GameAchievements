import { resultsHttp } from '@/api/http'
import type {
  CreateResultRequest,
  ResultRecordedResponse,
  ResultResponse,
} from '@/api/dto'

export const resultsApi = {
  create: async (request: CreateResultRequest) => {
    const { data } = await resultsHttp.post<ResultRecordedResponse>('/results', request)
    return data
  },
  getMine: async () => {
    const { data } = await resultsHttp.get<ResultResponse[]>('/results/me')
    return data
  },
}
