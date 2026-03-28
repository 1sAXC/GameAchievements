interface EmptyStateProps {
  title: string
  description: string
}

export const EmptyState = ({ title, description }: EmptyStateProps) => {
  return (
    <div className="state-card state-empty">
      <h3>{title}</h3>
      <p>{description}</p>
    </div>
  )
}
