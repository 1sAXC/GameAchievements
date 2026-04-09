using MassTransit;
using NotificationService.Clients;
using NotificationService.Data;
using NotificationService.Entities;
using NotificationService.Services;
using Shared.Contracts;

namespace NotificationService.Consumers;

public sealed class AchievementAwardedConsumer(
    AuthUsersClient authUsersClient,
    NotificationDbContext dbContext,
    SmtpEmailSender smtpEmailSender,
    ILogger<AchievementAwardedConsumer> logger) : IConsumer<AchievementAwarded>
{
    public async Task Consume(ConsumeContext<AchievementAwarded> context)
    {
        var message = context.Message;
        var cancellationToken = context.CancellationToken;

        logger.LogInformation(
            "AchievementAwarded received for user {UserId}, achievement {AchievementCode}, event {EventId}",
            message.UserId,
            message.AchievementCode,
            message.EventId);

        var email = await authUsersClient.GetUserEmailAsync(message.UserId, cancellationToken);

        var notification = new EmailNotification
        {
            Id = Guid.NewGuid(),
            EventId = message.EventId,
            UserId = message.UserId,
            Email = email,
            AchievementName = message.AchievementName,
            CreatedAtUtc = DateTime.UtcNow
        };

        dbContext.EmailNotifications.Add(notification);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Notification saved for user {UserId}, event {EventId}, email {Email}",
            notification.UserId,
            notification.EventId,
            notification.Email);

        await smtpEmailSender.SendAchievementAwardedEmailAsync(
            notification.Email,
            notification.AchievementName,
            cancellationToken);

        logger.LogInformation(
            "Achievement email sent for user {UserId}, event {EventId}, email {Email}",
            notification.UserId,
            notification.EventId,
            notification.Email);
    }
}
