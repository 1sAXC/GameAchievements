using AchievementsService.Data;
using AchievementsService.Domain;
using AchievementsService.Entities;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts;

namespace AchievementsService.Consumers;

public sealed class ResultRecordedConsumer(AchievementsDbContext dbContext, ILogger<ResultRecordedConsumer> logger) : IConsumer<ResultRecorded>
{
    public async Task Consume(ConsumeContext<ResultRecorded> context)
    {
        var result = context.Message;
        var now = DateTime.UtcNow;
        var cancellationToken = context.CancellationToken;

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        var insertedProcessedResult = await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"""
             INSERT INTO processed_results (result_id, processed_at)
             VALUES ({result.ResultId}, {now})
             ON CONFLICT (result_id) DO NOTHING
             """,
            cancellationToken);

        if (insertedProcessedResult == 0)
        {
            logger.LogInformation("Duplicate ResultRecorded ignored: {ResultId}", result.ResultId);
            await transaction.RollbackAsync(cancellationToken);
            return;
        }

        var userStats = await dbContext.UserStats.SingleOrDefaultAsync(x => x.UserId == result.UserId, cancellationToken);
        if (userStats is null)
        {
            userStats = new UserStat
            {
                UserId = result.UserId
            };
            dbContext.UserStats.Add(userStats);
        }

        userStats.GamesPlayed += 1;
        userStats.Wins += result.Win ? 1 : 0;
        userStats.TotalScore += result.Score;
        userStats.TotalKills += result.Kills;

        await dbContext.SaveChangesAsync(cancellationToken);

        foreach (var achievementCode in ResolveAchievementCodes(result, userStats))
        {
            await dbContext.Database.ExecuteSqlInterpolatedAsync(
                $"""
                 INSERT INTO user_achievements (id, user_id, achievement_code, earned_at)
                 VALUES ({Guid.NewGuid()}, {result.UserId}, {achievementCode}, {now})
                 ON CONFLICT (user_id, achievement_code) DO NOTHING
                 """,
                cancellationToken);
        }

        await transaction.CommitAsync(cancellationToken);
    }

    private static IReadOnlyCollection<string> ResolveAchievementCodes(ResultRecorded result, UserStat userStats)
    {
        var codes = new HashSet<string>(StringComparer.Ordinal);

        if (userStats.GamesPlayed >= 1) codes.Add(AchievementCodes.FirstGame);
        if (userStats.Wins >= 1) codes.Add(AchievementCodes.FirstWin);
        if (result.Score >= 500) codes.Add(AchievementCodes.Score500);
        if (result.Score >= 1000) codes.Add(AchievementCodes.Score1000);
        if (result.Kills >= 10) codes.Add(AchievementCodes.Kills10);
        if (result.IsPerfect) codes.Add(AchievementCodes.PerfectGame);

        if (userStats.GamesPlayed >= 10) codes.Add(AchievementCodes.Play10Games);
        if (userStats.Wins >= 5) codes.Add(AchievementCodes.Win5);
        if (userStats.TotalScore >= 5000) codes.Add(AchievementCodes.TotalScore5000);
        if (userStats.TotalKills >= 100) codes.Add(AchievementCodes.Kills100);

        return codes;
    }
}
