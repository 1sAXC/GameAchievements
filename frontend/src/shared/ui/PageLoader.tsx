interface PageLoaderProps {
  label?: string
}

export const PageLoader = ({ label = 'Загрузка...' }: PageLoaderProps) => {
  return (
    <div className="state-card state-loader" role="status" aria-live="polite">
      <div className="loader-orbit" aria-hidden="true" />
      <p>{label}</p>
    </div>
  )
}
