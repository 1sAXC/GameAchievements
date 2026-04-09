using MassTransit;
using Microsoft.EntityFrameworkCore;
using NotificationService.Clients;
using NotificationService.Configuration;
using NotificationService.Consumers;
using NotificationService.Data;
using NotificationService.Services;
using Shared.Contracts;

var builder = WebApplication.CreateBuilder(args);

var notificationsConnection = builder.Configuration.GetConnectionString("NotificationsDb")
                              ?? throw new InvalidOperationException("Connection string 'NotificationsDb' is missing.");
builder.Services.AddDbContext<NotificationDbContext>(options => options.UseNpgsql(notificationsConnection));

var authService = builder.Configuration.GetSection("AuthService").Get<AuthServiceOptions>()
                  ?? throw new InvalidOperationException("AuthService settings are missing.");
builder.Services.AddSingleton(authService);
builder.Services.AddHttpClient<AuthUsersClient>(client => AuthUsersClient.ConfigureHttpClient(client, authService));

var rabbitMq = builder.Configuration.GetSection("RabbitMq").Get<RabbitMqOptions>()
               ?? throw new InvalidOperationException("RabbitMq settings are missing.");
builder.Services.AddMassTransit(cfg =>
{
    cfg.AddConsumer<AchievementAwardedConsumer>();

    cfg.UsingRabbitMq((context, rabbitCfg) =>
    {
        var virtualHost = rabbitMq.VirtualHost.Trim('/');
        var hostUri = string.IsNullOrEmpty(virtualHost)
            ? $"rabbitmq://{rabbitMq.Host}:{rabbitMq.Port}/"
            : $"rabbitmq://{rabbitMq.Host}:{rabbitMq.Port}/{virtualHost}";

        rabbitCfg.Host(new Uri(hostUri), host =>
        {
            host.Username(rabbitMq.Username);
            host.Password(rabbitMq.Password);
        });

        rabbitCfg.Message<AchievementAwarded>(messageCfg => messageCfg.SetEntityName("achievements.awarded"));

        rabbitCfg.ReceiveEndpoint("notifications.achievement-awarded", endpointCfg =>
        {
            endpointCfg.ConfigureConsumer<AchievementAwardedConsumer>(context);
        });
    });
});

var smtp = builder.Configuration.GetSection("Smtp").Get<SmtpOptions>()
           ?? throw new InvalidOperationException("Smtp settings are missing.");
builder.Services.AddSingleton(smtp);
builder.Services.AddTransient<SmtpEmailSender>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();
    dbContext.Database.Migrate();
}

app.Run();
