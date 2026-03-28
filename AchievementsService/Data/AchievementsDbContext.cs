using AchievementsService.Domain;
using AchievementsService.Entities;
using Microsoft.EntityFrameworkCore;

namespace AchievementsService.Data;

public sealed class AchievementsDbContext(DbContextOptions<AchievementsDbContext> options) : DbContext(options)
{
    public DbSet<Achievement> Achievements => Set<Achievement>();
    public DbSet<UserAchievement> UserAchievements => Set<UserAchievement>();
    public DbSet<UserStat> UserStats => Set<UserStat>();
    public DbSet<ProcessedResult> ProcessedResults => Set<ProcessedResult>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Achievement>(entity =>
        {
            entity.ToTable("achievements");
            entity.HasKey(x => x.Code);

            entity.Property(x => x.Code).HasColumnName("code").HasMaxLength(50);
            entity.Property(x => x.Name).HasColumnName("name").HasMaxLength(150).IsRequired();
            entity.Property(x => x.Description).HasColumnName("description").HasMaxLength(500).IsRequired();
            entity.Property(x => x.Type).HasColumnName("type").HasMaxLength(30).IsRequired();
            entity.Property(x => x.TargetValue).HasColumnName("target_value");

            entity.HasData(
                new Achievement { Code = AchievementCodes.FirstGame, Name = "First Game", Description = "Complete your first game.", Type = "one_time", TargetValue = 1 },
                new Achievement { Code = AchievementCodes.FirstWin, Name = "First Win", Description = "Win your first game.", Type = "one_time", TargetValue = 1 },
                new Achievement { Code = AchievementCodes.Score500, Name = "Score 500", Description = "Reach 500 score in a single game.", Type = "one_time", TargetValue = 500 },
                new Achievement { Code = AchievementCodes.Score1000, Name = "Score 1000", Description = "Reach 1000 score in a single game.", Type = "one_time", TargetValue = 1000 },
                new Achievement { Code = AchievementCodes.Kills10, Name = "Kills 10", Description = "Make 10 kills in a single game.", Type = "one_time", TargetValue = 10 },
                new Achievement { Code = AchievementCodes.PerfectGame, Name = "Perfect Game", Description = "Win a game with zero deaths.", Type = "one_time", TargetValue = 1 },
                new Achievement { Code = AchievementCodes.Play10Games, Name = "Play 10 Games", Description = "Play 10 games in total.", Type = "progressive", TargetValue = 10 },
                new Achievement { Code = AchievementCodes.Win5, Name = "Win 5 Games", Description = "Win 5 games in total.", Type = "progressive", TargetValue = 5 },
                new Achievement { Code = AchievementCodes.TotalScore5000, Name = "Total Score 5000", Description = "Accumulate 5000 score in total.", Type = "progressive", TargetValue = 5000 },
                new Achievement { Code = AchievementCodes.Kills100, Name = "Kills 100", Description = "Accumulate 100 kills in total.", Type = "progressive", TargetValue = 100 }
            );
        });

        modelBuilder.Entity<UserAchievement>(entity =>
        {
            entity.ToTable("user_achievements");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.UserId).HasColumnName("user_id").IsRequired();
            entity.Property(x => x.AchievementCode).HasColumnName("achievement_code").HasMaxLength(50).IsRequired();
            entity.Property(x => x.EarnedAt).HasColumnName("earned_at").IsRequired();

            entity.HasIndex(x => new { x.UserId, x.AchievementCode }).IsUnique();
            entity.HasOne(x => x.Achievement)
                .WithMany()
                .HasForeignKey(x => x.AchievementCode)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UserStat>(entity =>
        {
            entity.ToTable("user_stats");
            entity.HasKey(x => x.UserId);

            entity.Property(x => x.UserId).HasColumnName("user_id");
            entity.Property(x => x.GamesPlayed).HasColumnName("games_played").IsRequired();
            entity.Property(x => x.Wins).HasColumnName("wins").IsRequired();
            entity.Property(x => x.TotalScore).HasColumnName("total_score").IsRequired();
            entity.Property(x => x.TotalKills).HasColumnName("total_kills").IsRequired();
        });

        modelBuilder.Entity<ProcessedResult>(entity =>
        {
            entity.ToTable("processed_results");
            entity.HasKey(x => x.ResultId);

            entity.Property(x => x.ResultId).HasColumnName("result_id");
            entity.Property(x => x.ProcessedAt).HasColumnName("processed_at").IsRequired();
        });
    }
}
