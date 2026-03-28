using Microsoft.EntityFrameworkCore;
using ResultsService.Entities;

namespace ResultsService.Data;

public sealed class ResultsDbContext(DbContextOptions<ResultsDbContext> options) : DbContext(options)
{
    public DbSet<GameResult> GameResults => Set<GameResult>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GameResult>(entity =>
        {
            entity.ToTable("game_results");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.UserId).HasColumnName("user_id").IsRequired();
            entity.Property(x => x.GameId).HasColumnName("game_id").HasMaxLength(100).IsRequired();
            entity.Property(x => x.Score).HasColumnName("score").IsRequired();
            entity.Property(x => x.Kills).HasColumnName("kills").IsRequired();
            entity.Property(x => x.Deaths).HasColumnName("deaths").IsRequired();
            entity.Property(x => x.IsWin).HasColumnName("is_win").IsRequired();
            entity.Property(x => x.IsPerfect).HasColumnName("is_perfect").IsRequired();
            entity.Property(x => x.PlayedAt).HasColumnName("played_at").IsRequired();
            entity.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();

            entity.HasIndex(x => x.UserId);
        });
    }
}
