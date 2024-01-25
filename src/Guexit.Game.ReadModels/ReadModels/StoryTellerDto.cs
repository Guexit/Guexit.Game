namespace Guexit.Game.ReadModels.ReadModels;

public sealed class StoryTellerDto
{
    public required string Username { get; init; }
    public required string Nickname { get; init; }
    public required string PlayerId { get; init; }
    public required string? Story { get; init; }
}