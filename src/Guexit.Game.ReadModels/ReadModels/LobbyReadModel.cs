﻿namespace Guexit.Game.ReadModels.ReadModels;

public sealed class LobbyReadModel
{
    public required Guid GameRoomId { get; init; }
    public required int RequiredMinPlayers { get; init; }
    public required LobbyPlayerDto[] Players { get; init; }
    public required bool CanStartGame { get; init; }
    public required bool IsPublic { get; init; }
    public required string GameStatus { get; init; }
    public required LobbyPlayerDto Creator { get; init; }
}

public sealed class LobbyPlayerDto
{
    public required string Id { get; init; }
    public required string Username { get; init; }
    public required string Nickname { get; init; }
}
