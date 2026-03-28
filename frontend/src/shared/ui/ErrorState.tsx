interface ErrorStateProps {
  title?: string
  message: string
  actionLabel?: string
  onAction?: () => void
}

export const ErrorState = ({
  title = 'Что-то пошло не так',
  message,
  actionLabel,
  onAction,
}: ErrorStateProps) => {
  return (
    <div className="state-card state-error" role="alert">
      <h3>{title}</h3>
      <p>{message}</p>
      {actionLabel && onAction ? (
        <button type="button" onClick={onAction}>
          {actionLabel}
        </button>
      ) : null}
    </div>
  )
}
