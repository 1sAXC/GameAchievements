using System.Security.Claims;
using AchievementsService.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts;

namespace AchievementsService.Controllers;

[ApiController]
[Route("users")]
[Authorize]
public sealed class UsersController(AchievementsDbContext dbContext) : ControllerBase
{
    [HttpGet("me/achievements")]
    [ProducesResponseType<List<Achievement>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<Achievement>>> GetMyAchievements(CancellationToken cancellationToken)
    {
        var userIdRaw = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (!Guid.TryParse(userIdRaw, out var userId))
        {
            return Unauthorized();
        }

        var achievements = await dbContext.UserAchievements
            .Where(x => x.UserId == userId)
            .Join(
                dbContext.Achievements,
                userAchievement => userAchievement.AchievementCode,
                achievement => achievement.Code,
                (userAchievement, achievement) => new
                {
                    userAchievement.EarnedAt,
                    achievement.Code,
                    achievement.Name,
                    achievement.Description
                })
            .OrderByDescending(x => x.EarnedAt)
            .Select(x => new Achievement(
                x.Code,
                x.Name,
                x.Description,
                x.EarnedAt))
            .ToListAsync(cancellationToken);

        return Ok(achievements);
    }
}
