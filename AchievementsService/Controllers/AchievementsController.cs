using AchievementsService.Contracts;
using AchievementsService.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AchievementsService.Controllers;

[ApiController]
[Route("achievements")]
public sealed class AchievementsController(AchievementsDbContext dbContext) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<List<AchievementDefinitionResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<AchievementDefinitionResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var achievements = await dbContext.Achievements
            .OrderBy(x => x.Code)
            .Select(x => new AchievementDefinitionResponse(x.Code, x.Name, x.Description, x.Type, x.TargetValue))
            .ToListAsync(cancellationToken);

        return Ok(achievements);
    }
}
