# GameAchievements

## Unified JWT rules

All services use the same JWT settings (`Issuer`, `Audience`, `SigningKey`):

- `AuthService` issues access tokens in `POST /auth/token`.
- `ResultsService` validates bearer tokens and accepts only tokens from `AuthService`.
- `AchievementsService` validates bearer tokens and accepts only tokens from `AuthService`.

JWT config is stored in each service `appsettings.json` under `Jwt`.

## Contracts for results and achievements

`CreateResultRequest` contains:

- `GameId`
- `Score`
- `Kills`
- `Deaths`
- `Win`
- `PlayedAt`

`ResultRecorded` contains:

- `ResultId` (required unique id for deduplication)
- `UserId`
- `GameId`
- `Score`
- `Kills`
- `Deaths`
- `Win`
- `IsPerfect`
- `PlayedAt`

## PERFECT_GAME rule

For this project, `PERFECT_GAME` is defined as:

- `Win == true`
- `Deaths == 0`

`ResultsService` computes `IsPerfect` using this rule and includes it in `ResultRecorded`.
`AchievementsService` awards `PERFECT_GAME` only when `IsPerfect == true`.

## Deduplication rule

`AchievementsService` deduplicates processed results by `ResultId` (in-memory `processedResults` set).
If a duplicated `ResultId` is received, achievements are not recalculated.
