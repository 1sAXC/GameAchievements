namespace NotificationService.Entities;

public sealed class EmailNotification
{
    public Guid Id { get; set; }
    public Guid EventId { get; set; }
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string AchievementName { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
}
