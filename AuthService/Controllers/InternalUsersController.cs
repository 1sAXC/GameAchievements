using AuthService.Configuration;
using AuthService.Contracts;
using AuthService.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Controllers;

[ApiController]
[Route("internal/users")]
[ApiExplorerSettings(IgnoreApi = true)]
public sealed class InternalUsersController(
    AuthDbContext dbContext,
    InternalApiOptions internalApiOptions) : ControllerBase
{
    [HttpGet("{id:guid}/email")]
    [ProducesResponseType<InternalUserEmailResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<InternalUserEmailResponse>> GetEmail([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        if (!Request.Headers.TryGetValue(internalApiOptions.HeaderName, out var apiKey) ||
            !StringValuesEquals(apiKey, internalApiOptions.ApiKey))
        {
            return Unauthorized();
        }

        var user = await dbContext.Users
            .Where(x => x.Id == id)
            .Select(x => new InternalUserEmailResponse(x.Id, x.Email))
            .SingleOrDefaultAsync(cancellationToken);

        return user is null ? NotFound() : Ok(user);
    }

    private static bool StringValuesEquals(Microsoft.Extensions.Primitives.StringValues actualValues, string expectedValue)
    {
        if (string.IsNullOrWhiteSpace(expectedValue))
        {
            return false;
        }

        return actualValues.Count == 1 && string.Equals(actualValues[0], expectedValue, StringComparison.Ordinal);
    }
}
