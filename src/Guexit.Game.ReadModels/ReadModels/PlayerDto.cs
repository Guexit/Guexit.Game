namespace Guexit.Game.ReadModels.ReadModels;

public sealed class PlayerDto
{
    public required string PlayerId { get; init; }
    public required string Username { get; init; }
    public required string Nickname { get; init; }
}
