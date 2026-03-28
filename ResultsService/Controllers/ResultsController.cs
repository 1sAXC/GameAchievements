using System.Security.Claims;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResultsService.Clients;
using ResultsService.Contracts;
using ResultsService.Data;
using ResultsService.Entities;
using Shared.Contracts;

namespace ResultsService.Controllers;

[ApiController]
[Route("results")]
[Authorize]
public sealed class ResultsController(ResultsDbContext dbContext, IPublishEndpoint publishEndpoint, AuthUsersClient authUsersClient) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<ResultRecorded>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<ResultRecorded>> RecordResult([FromBody] CreateResultRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.GameId))
        {
            return BadRequest("GameId is required.");
        }

        var userIdRaw = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (!Guid.TryParse(userIdRaw, out var userId))
        {
            return Unauthorized();
        }

        bool userExists;
        try
        {
            userExists = await authUsersClient.UserExistsAsync(userId, cancellationToken);
        }
        catch (HttpRequestException)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, "Auth service is unavailable.");
        }
        catch (TaskCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, "Auth service timeout.");
        }

        if (!userExists)
        {
            return Unauthorized();
        }

        var playedAt = request.PlayedAt == default ? DateTime.UtcNow : request.PlayedAt;
        var isPerfect = request.Win && request.Deaths == 0;

        var gameResult = new GameResult
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            GameId = request.GameId,
            Score = request.Score,
            Kills = request.Kills,
            Deaths = request.Deaths,
            IsWin = request.Win,
            IsPerfect = isPerfect,
            PlayedAt = playedAt,
            CreatedAt = DateTime.UtcNow
        };

        dbContext.GameResults.Add(gameResult);
        await dbContext.SaveChangesAsync(cancellationToken);

        var recorded = new ResultRecorded(
            ResultId: gameResult.Id,
            UserId: userId,
            GameId: request.GameId,
            Score: request.Score,
            Kills: request.Kills,
            Deaths: request.Deaths,
            Win: request.Win,
            IsPerfect: isPerfect,
            PlayedAt: playedAt);

        await publishEndpoint.Publish(recorded, cancellationToken);

        return StatusCode(StatusCodes.Status201Created, recorded);
    }

    [HttpGet("me")]
    [ProducesResponseType<List<ResultResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<ResultResponse>>> GetMyResults(CancellationToken cancellationToken)
    {
        var userIdRaw = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (!Guid.TryParse(userIdRaw, out var userId))
        {
            return Unauthorized();
        }

        var results = await dbContext.GameResults
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new ResultResponse(
                x.Id,
                x.UserId,
                x.GameId,
                x.Score,
                x.Kills,
                x.Deaths,
                x.IsWin,
                x.IsPerfect,
                x.PlayedAt,
                x.CreatedAt))
            .ToListAsync(cancellationToken);

        return Ok(results);
    }
}
