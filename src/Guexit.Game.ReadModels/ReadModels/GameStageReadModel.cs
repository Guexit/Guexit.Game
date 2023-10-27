namespace Guexit.Game.ReadModels.ReadModels;

public sealed class GameStageReadModel
{
    public required Guid GameRoomId { get; init; }
    public required string CurrentStage { get; init; }
}

public sealed class GameStage
{
    public static readonly GameStage Lobby = new("Lobby");
    public static readonly GameStage Board = new("Board");
    public static readonly GameStage Voting = new("Voting");
    public static readonly GameStage End = new("End");
    
    public string Value { get; }

    private GameStage(string value)
    {
        Value = value;
    }
}