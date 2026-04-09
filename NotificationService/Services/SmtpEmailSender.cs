using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using NotificationService.Configuration;

namespace NotificationService.Services;

public sealed class SmtpEmailSender(SmtpOptions smtpOptions, ILogger<SmtpEmailSender> logger)
{
    public async Task SendAchievementAwardedEmailAsync(string email, string achievementName, CancellationToken cancellationToken)
    {
        ValidateConfiguration();

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(smtpOptions.FromName, smtpOptions.FromEmail));
        message.To.Add(MailboxAddress.Parse(email));
        message.Subject = $"Вы получили достижение: {achievementName}";
        message.Body = new BodyBuilder
        {
            TextBody = BuildTextBody(achievementName),
            HtmlBody = BuildHtmlBody(achievementName)
        }.ToMessageBody();

        using var client = new SmtpClient();

        logger.LogInformation(
            "Sending achievement email to {Email} via {Host}:{Port} as {Username}",
            email,
            smtpOptions.Host,
            smtpOptions.Port,
            smtpOptions.Username);

        await client.ConnectAsync(smtpOptions.Host, smtpOptions.Port, SecureSocketOptions.StartTls, cancellationToken);
        await client.AuthenticateAsync(smtpOptions.Username, smtpOptions.Password, cancellationToken);
        await client.SendAsync(message, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);
    }

    private void ValidateConfiguration()
    {
        if (string.IsNullOrWhiteSpace(smtpOptions.Host) ||
            string.IsNullOrWhiteSpace(smtpOptions.Username) ||
            string.IsNullOrWhiteSpace(smtpOptions.Password) ||
            string.IsNullOrWhiteSpace(smtpOptions.FromEmail))
        {
            throw new InvalidOperationException("SMTP settings are incomplete.");
        }
    }

    private static string BuildTextBody(string achievementName)
    {
        return $"Поздравляем!{Environment.NewLine}{Environment.NewLine}Вы получили достижение: {achievementName}";
    }

    private static string BuildHtmlBody(string achievementName)
    {
        return $"""
                <html>
                <body>
                    <p>Поздравляем!</p>
                    <p>Вы получили достижение: <strong>{System.Net.WebUtility.HtmlEncode(achievementName)}</strong></p>
                </body>
                </html>
                """;
    }
}
