import { useState } from 'react'
import axios from 'axios'
import type { CreateResultRequest, ResultRecordedResponse, ResultResponse } from '@/api/dto'
import { resultsApi } from '@/api/results.api'
import { EmptyState } from '@/shared/ui/EmptyState'
import { ErrorState } from '@/shared/ui/ErrorState'

const gameIds = ['arena-1', 'arena-2', 'survival-1', 'duel-7']

const randomInt = (min: number, max: number) =>
  Math.floor(Math.random() * (max - min + 1)) + min

const createRandomRequest = (): CreateResultRequest => ({
  gameId: gameIds[randomInt(0, gameIds.length - 1)],
  score: randomInt(0, 5000),
  kills: randomInt(0, 35),
  deaths: randomInt(0, 20),
  win: Math.random() > 0.4,
  playedAt: new Date().toISOString(),
})

export const GamePage = () => {
  const [resultDraft, setResultDraft] = useState<CreateResultRequest>({
    gameId: 'arena-1',
    score: 0,
    kills: 0,
    deaths: 0,
    win: false,
    playedAt: new Date().toISOString(),
  })
  const [submitStatus, setSubmitStatus] = useState<string>('Готово')
  const [submitError, setSubmitError] = useState<string | null>(null)
  const [lastRecordedResult, setLastRecordedResult] = useState<ResultRecordedResponse | null>(null)
  const [myResults, setMyResults] = useState<ResultResponse[]>([])
  const [isSubmitting, setIsSubmitting] = useState(false)
  const [isLoadingResults, setIsLoadingResults] = useState(false)

  const updateDraft = (patch: Partial<CreateResultRequest>) => {
    setResultDraft((current) => ({
      ...current,
      ...patch,
    }))
  }

  const onRandomize = () => {
    setSubmitStatus('Случайные значения сгенерированы')
    setSubmitError(null)
    setResultDraft(createRandomRequest())
  }

  const finishGame = async () => {
    const request: CreateResultRequest = {
      ...resultDraft,
      gameId: resultDraft.gameId.trim(),
      playedAt: new Date().toISOString(),
    }

    setIsSubmitting(true)
    setSubmitError(null)
    setSubmitStatus('Отправляем результат в /results...')

    try {
      const response = await resultsApi.create(request)
      setLastRecordedResult(response)
      setResultDraft(request)
      setSubmitStatus('Результат успешно сохранён')
    } catch (error) {
      if (axios.isAxiosError(error)) {
        const status = error.response?.status
        if (status === 401) {
          setSubmitError('Не авторизованы. Пожалуйста, войдите снова.')
        } else if (status === 400) {
          setSubmitError('Некорректный payload результата. Проверьте gameId и метрики.')
        } else {
          setSubmitError(`Не удалось сохранить результат (${status ?? 'ошибка сети'}).`)
        }
      } else {
        setSubmitError('Не удалось сохранить результат.')
      }

      setSubmitStatus('Ошибка')
    } finally {
      setIsSubmitting(false)
    }
  }

  const loadMyResults = async () => {
    setIsLoadingResults(true)
    setSubmitError(null)

    try {
      const results = await resultsApi.getMine()
      setMyResults(results)
      setSubmitStatus(`Загружено записей из /results/me: ${results.length}`)
    } catch (error) {
      if (axios.isAxiosError(error)) {
        setSubmitError(`Не удалось загрузить /results/me (${error.response?.status ?? 'ошибка сети'}).`)
      } else {
        setSubmitError('Не удалось загрузить /results/me.')
      }
    } finally {
      setIsLoadingResults(false)
    }
  }

  return (
    <section className="surface mission-surface">
      <div className="mission-header">
        <h1>Игровая консоль</h1>
        <p className="hint">Сгенерируйте результат матча и отправьте его в ResultsService.</p>
      </div>

      <div className="mission-grid">
        <section className="panel-card">
          <h2>Управление</h2>
          <label htmlFor="gameIdInput">ID игры</label>
          <input
            id="gameIdInput"
            value={resultDraft.gameId}
            onChange={(event) => updateDraft({ gameId: event.target.value })}
            aria-describedby="gameIdHelp"
          />
          <p id="gameIdHelp" className="hint">
            Используйте стабильный ID режима, например <code>arena-1</code> или{' '}
            <code>survival-1</code>.
          </p>

          <div className="actions">
            <button
              type="button"
              onClick={() =>
                updateDraft({ kills: resultDraft.kills + 1, score: resultDraft.score + 100 })
              }
            >
              Враг повержен (+убийство)
            </button>
            <button
              type="button"
              onClick={() =>
                updateDraft({
                  deaths: resultDraft.deaths + 1,
                  score: Math.max(0, resultDraft.score - 50),
                })
              }
            >
              Корабль повреждён (+смерть)
            </button>
            <button
              type="button"
              onClick={() => updateDraft({ score: resultDraft.score + 250 })}
            >
              Комбо-буст
            </button>
            <button type="button" onClick={() => updateDraft({ win: !resultDraft.win })}>
              Переключить победу: {resultDraft.win ? 'Да' : 'Нет'}
            </button>
          </div>
        </section>

        <section className="panel-card">
          <h2>Текущий черновик результата</h2>
          <div className="stats">
            <p className="stat-pill">Счёт: {resultDraft.score}</p>
            <p className="stat-pill">Убийства: {resultDraft.kills}</p>
            <p className="stat-pill">Смерти: {resultDraft.deaths}</p>
            <p className="stat-pill">Победа: {resultDraft.win ? 'да' : 'нет'}</p>
          </div>
          <div className="actions">
            <button type="button" onClick={onRandomize}>
              Сгенерировать случайные значения
            </button>
            <button type="button" onClick={finishGame} disabled={isSubmitting}>
              {isSubmitting ? 'Отправляем...' : 'Отправить POST /results'}
            </button>
            <button type="button" onClick={loadMyResults} disabled={isLoadingResults}>
              {isLoadingResults ? 'Загружаем...' : 'Загрузить GET /results/me'}
            </button>
          </div>
          <p className="hint">Статус: {submitStatus}</p>
        </section>
      </div>

      {submitError ? <ErrorState title="Запрос завершился ошибкой" message={submitError} /> : null}

      <div className="mission-grid">
        <section className="panel-card">
          <h2>Ответ по последнему результату</h2>
          {!lastRecordedResult ? (
            <EmptyState
              title="Результат ещё не отправлен"
              description="Завершите миссию и отправьте результат, чтобы увидеть подтверждение backend."
            />
          ) : (
            <div className="result-list">
              <p>
                <strong>ID результата:</strong> {lastRecordedResult.resultId}
              </p>
              <p>
                <strong>ID пользователя:</strong> {lastRecordedResult.userId}
              </p>
              <p>
                <strong>ID игры:</strong> {lastRecordedResult.gameId}
              </p>
              <p>
                <strong>Счёт:</strong> {lastRecordedResult.score}
              </p>
              <p>
                <strong>Убийства / Смерти:</strong> {lastRecordedResult.kills} /{' '}
                {lastRecordedResult.deaths}
              </p>
              <p>
                <strong>Победа / Идеальная:</strong> {lastRecordedResult.win ? 'да' : 'нет'} /{' '}
                {lastRecordedResult.isPerfect ? 'да' : 'нет'}
              </p>
              <p>
                <strong>Сыграно:</strong> {new Date(lastRecordedResult.playedAt).toLocaleString()}
              </p>
            </div>
          )}
        </section>

        <section className="panel-card">
          <h2>Недавняя история</h2>
          {myResults.length === 0 ? (
            <EmptyState
              title="Сохранённых миссий пока нет"
              description="Нажмите «Загрузить GET /results/me» после отправки результата."
            />
          ) : (
            <ul className="result-history">
              {myResults.slice(0, 5).map((result) => (
                <li key={result.id}>
                  <p>
                    <strong>{result.gameId}</strong> в {new Date(result.playedAt).toLocaleString()}
                  </p>
                  <p className="hint">
                    счёт {result.score} | убийства {result.kills} | смерти {result.deaths} |
                    победа {result.isWin ? 'да' : 'нет'}
                  </p>
                </li>
              ))}
            </ul>
          )}
        </section>
      </div>
    </section>
  )
}
