using System.Net.Http.Json;
using Guexit.Game.Component.IntegrationTests.Builders;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.ReadModels.ReadModels;
using Guexit.Game.Tests.Common;
using Guexit.Game.Tests.Common.Builders;

namespace Guexit.Game.Component.IntegrationTests;

public sealed class WhenQueryingGameLobby : ComponentTest
{
    public WhenQueryingGameLobby(GameWebApplicationFactory factory) : base(factory)
    { }

    [Fact]
    public async Task ReturnsLobbyReadModel()
    {
        var gameRoomId = new GameRoomId(Guid.NewGuid());
        var creatorId = new PlayerId("player1");
        var creatorUsername = "thanos";
        await Save(new GameRoomBuilder()
            .WithId(gameRoomId)
            .WithCreator(creatorId)
            .WithPlayersThatJoined("player2", "player3")
            .Build());
        await Save(
        [
            new PlayerBuilder().WithId(creatorId).WithUsername("thanos").Build(),
            new PlayerBuilder().WithId("player2").WithUsername("hulk").Build(),
            new PlayerBuilder().WithId("player3").WithUsername("ironman").Build()
        ]);

        using var response = await Send(HttpMethod.Get, $"game-rooms/{gameRoomId.Value}/lobby", creatorId);

        var lobbyReadModel = await response.Content.ReadFromJsonAsync<LobbyReadModel>();
        lobbyReadModel.Should().NotBeNull();
        lobbyReadModel!.GameRoomId.Should().Be(gameRoomId.Value);
        lobbyReadModel.Creator.Id.Should().Be(creatorId);
        lobbyReadModel.Creator.Username.Should().Be(creatorUsername);
        lobbyReadModel.CanStartGame.Should().BeTrue();
        lobbyReadModel.RequiredMinPlayers.Should().Be(3);
        lobbyReadModel.Players.Select(x => x.Id).Should().BeEquivalentTo("player1", "player2", "player3");
        lobbyReadModel.Players.Select(x => x.Username).Should().BeEquivalentTo("thanos", "hulk", "ironman");
        lobbyReadModel.GameStatus.Should().Be(GameStatus.NotStarted.Value);
    }
    
    [Fact]
    public async Task CannotStartIfItsNotTheCreatorOfTheGameRoom()
    {
        var gameRoomId = new GameRoomId(Guid.NewGuid());
        var creatorId = new PlayerId("player1");
        var nonCreatorId = new PlayerId("player2");
        await Save(new GameRoomBuilder()
            .WithId(gameRoomId)
            .WithCreator(creatorId)
            .WithPlayersThatJoined(nonCreatorId, "player3")
            .Build());
        await Save(
        [
            new PlayerBuilder().WithId("player1").WithUsername("thanos").Build(),
            new PlayerBuilder().WithId("player2").WithUsername("hulk").Build(),
            new PlayerBuilder().WithId("player3").WithUsername("ironman").Build()
        ]);

        using var response = await Send(HttpMethod.Get, $"game-rooms/{gameRoomId.Value}/lobby", authenticatedPlayerId: nonCreatorId);

        var lobbyReadModel = await response.Content.ReadFromJsonAsync<LobbyReadModel>();
        lobbyReadModel.Should().NotBeNull();
        lobbyReadModel!.CanStartGame.Should().BeFalse();
    }
}
