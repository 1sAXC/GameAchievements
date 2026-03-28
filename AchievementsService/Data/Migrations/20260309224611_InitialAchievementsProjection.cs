using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AchievementsService.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialAchievementsProjection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "achievements",
                columns: table => new
                {
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    target_value = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_achievements", x => x.code);
                });

            migrationBuilder.CreateTable(
                name: "processed_results",
                columns: table => new
                {
                    result_id = table.Column<Guid>(type: "uuid", nullable: false),
                    processed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_processed_results", x => x.result_id);
                });

            migrationBuilder.CreateTable(
                name: "user_stats",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    games_played = table.Column<int>(type: "integer", nullable: false),
                    wins = table.Column<int>(type: "integer", nullable: false),
                    total_score = table.Column<int>(type: "integer", nullable: false),
                    total_kills = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_stats", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "user_achievements",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    achievement_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    earned_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_achievements", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_achievements_achievements_achievement_code",
                        column: x => x.achievement_code,
                        principalTable: "achievements",
                        principalColumn: "code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "achievements",
                columns: new[] { "code", "description", "name", "target_value", "type" },
                values: new object[,]
                {
                    { "FIRST_GAME", "Complete your first game.", "First Game", 1, "one_time" },
                    { "FIRST_WIN", "Win your first game.", "First Win", 1, "one_time" },
                    { "KILLS_10", "Make 10 kills in a single game.", "Kills 10", 10, "one_time" },
                    { "KILLS_100", "Accumulate 100 kills in total.", "Kills 100", 100, "progressive" },
                    { "PERFECT_GAME", "Win a game with zero deaths.", "Perfect Game", 1, "one_time" },
                    { "PLAY_10_GAMES", "Play 10 games in total.", "Play 10 Games", 10, "progressive" },
                    { "SCORE_1000", "Reach 1000 score in a single game.", "Score 1000", 1000, "one_time" },
                    { "SCORE_500", "Reach 500 score in a single game.", "Score 500", 500, "one_time" },
                    { "TOTAL_SCORE_5000", "Accumulate 5000 score in total.", "Total Score 5000", 5000, "progressive" },
                    { "WIN_5", "Win 5 games in total.", "Win 5 Games", 5, "progressive" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_user_achievements_achievement_code",
                table: "user_achievements",
                column: "achievement_code");

            migrationBuilder.CreateIndex(
                name: "IX_user_achievements_user_id_achievement_code",
                table: "user_achievements",
                columns: new[] { "user_id", "achievement_code" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "processed_results");

            migrationBuilder.DropTable(
                name: "user_achievements");

            migrationBuilder.DropTable(
                name: "user_stats");

            migrationBuilder.DropTable(
                name: "achievements");
        }
    }
}
