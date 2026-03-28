import { useEffect, useState } from 'react'
import type { AchievementDto, AuthUserResponse, ResultResponse } from '@/api/dto'
import { achievementsApi } from '@/api/achievements.api'
import { authApi } from '@/api/auth.api'
import { resultsApi } from '@/api/results.api'
import { useAuthStore } from '@/features/auth/model/auth.store'
import { EmptyState } from '@/shared/ui/EmptyState'
import { ErrorState } from '@/shared/ui/ErrorState'
import { PageLoader } from '@/shared/ui/PageLoader'

const formatDate = (value: string) => new Date(value).toLocaleString()

export const ProfilePage = () => {
  const { setUser } = useAuthStore()
  const [profile, setProfile] = useState<AuthUserResponse | null>(null)
  const [results, setResults] = useState<ResultResponse[]>([])
  const [achievements, setAchievements] = useState<AchievementDto[]>([])
  const [profileError, setProfileError] = useState<string | null>(null)
  const [resultsError, setResultsError] = useState<string | null>(null)
  const [achievementsError, setAchievementsError] = useState<string | null>(null)
  const [isRefreshing, setIsRefreshing] = useState(false)
  const [isFirstLoad, setIsFirstLoad] = useState(true)

  const refreshProfile = async () => {
    setProfileError(null)
    setResultsError(null)
    setAchievementsError(null)
    setIsRefreshing(true)

    const [meResponse, resultsResponse, achievementsResponse] = await Promise.allSettled([
      authApi.me(),
      resultsApi.getMine(),
      achievementsApi.getMyAchievements(),
    ])

    if (meResponse.status === 'fulfilled') {
      const me = meResponse.value
      setProfile(me)
      setUser(me)
    } else {
      const message =
        meResponse.reason instanceof Error
          ? meResponse.reason.message
          : 'Не удалось загрузить данные профиля'
      setProfileError(message)
    }

    if (resultsResponse.status === 'fulfilled') {
      setResults(resultsResponse.value)
    } else {
      const message =
        resultsResponse.reason instanceof Error
          ? resultsResponse.reason.message
          : 'Не удалось загрузить результаты'
      setResultsError(message)
    }

    if (achievementsResponse.status === 'fulfilled') {
      setAchievements(achievementsResponse.value)
    } else {
      const message =
        achievementsResponse.reason instanceof Error
          ? achievementsResponse.reason.message
          : 'Не удалось загрузить достижения'
      setAchievementsError(message)
    }

    setIsRefreshing(false)
    setIsFirstLoad(false)
  }

  useEffect(() => {
    void refreshProfile()
  }, [])

  return (
    <section className="surface profile-surface">
      <div className="mission-header">
        <h1>Профиль игрока</h1>
        <p className="hint">Единые данные из сервисов Auth, Results и Achievements.</p>
      </div>

      <div className="actions">
        <button type="button" onClick={refreshProfile} disabled={isRefreshing}>
          {isRefreshing ? 'Обновляем...' : 'Обновить данные профиля'}
        </button>
      </div>

      {isFirstLoad && isRefreshing ? <PageLoader label="Загружаем данные профиля..." /> : null}

      {profileError ? (
        <ErrorState
          title="Профиль недоступен"
          message={profileError}
          actionLabel="Повторить"
          onAction={refreshProfile}
        />
      ) : null}

      <section className="panel-card">
        <h2>Личные данные</h2>
        <div className="identity-grid">
          <p className="stat-pill">Эл. почта: {profile?.email ?? '-'}</p>
          <p className="stat-pill">
            Зарегистрирован: {profile?.createdAtUtc ? formatDate(profile.createdAtUtc) : '-'}
          </p>
          <p className="stat-pill">ID пользователя: {profile?.id ?? '-'}</p>
        </div>
      </section>

      <section className="panel-card">
        <h2>Результаты матчей</h2>
        {resultsError ? (
          <ErrorState title="Ошибка загрузки результатов" message={resultsError} />
        ) : results.length === 0 ? (
          <EmptyState
            title="Результатов пока нет"
            description="Сыграйте матч, чтобы получить первую запись."
          />
        ) : (
          <div className="results-grid">
            {results.map((result) => (
              <article className="result-tile" key={result.id}>
                <p className="tile-title">{result.gameId}</p>
                <p className="hint">{formatDate(result.playedAt)}</p>
                <div className="tile-metrics">
                  <span>Счёт {result.score}</span>
                  <span>Убийства {result.kills}</span>
                  <span>Смерти {result.deaths}</span>
                </div>
                <p className="hint">
                  Победа: {result.isWin ? 'да' : 'нет'} | Идеальная:{' '}
                  {result.isPerfect ? 'да' : 'нет'}
                </p>
              </article>
            ))}
          </div>
        )}
      </section>

      <section className="panel-card">
        <h2>Достижения</h2>
        {achievementsError ? (
          <ErrorState title="Ошибка загрузки достижений" message={achievementsError} />
        ) : achievements.length === 0 ? (
          <EmptyState
            title="Достижений пока нет"
            description="Выполняйте цели в матчах, чтобы разблокировать награды."
          />
        ) : (
          <div className="achievements-grid">
            {achievements.map((achievement) => (
              <article
                className="achievement-tile"
                key={`${achievement.code}-${achievement.grantedAt}`}
              >
                <p className="tile-title">{achievement.name}</p>
                <p className="hint">{achievement.description}</p>
                <p className="tag">{achievement.code}</p>
                <p className="hint">Получено: {formatDate(achievement.grantedAt)}</p>
              </article>
            ))}
          </div>
        )}
      </section>
    </section>
  )
}
