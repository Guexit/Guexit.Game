using System.Net.Http.Json;
using Guexit.Game.Component.IntegrationTests.Builders;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.ReadModels.ReadModels;
using Guexit.Game.Tests.Common;

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
        await Save(new GameRoomBuilder()
            .WithId(gameRoomId)
            .WithCreator(creatorId)
            .WithPlayersThatJoined("player2", "player3")
            .WithMinRequiredPlayers(3)
            .Build());
        await Save(new[]
        {
            new PlayerBuilder().WithId("player1").WithUsername("thanos").Build(),
            new PlayerBuilder().WithId("player2").WithUsername("hulk").Build(),
            new PlayerBuilder().WithId("player3").WithUsername("ironman").Build()
        });

        using var client = WebApplicationFactory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Get, $"game-rooms/{gameRoomId.Value}/lobby");
        request.AddPlayerIdHeader(creatorId);
        var response = await client.SendAsync(request);

        var lobbyReadModel = await response.Content.ReadFromJsonAsync<LobbyReadModel>();
        lobbyReadModel.Should().NotBeNull();
        lobbyReadModel!.GameRoomId.Should().Be(gameRoomId.Value);
        lobbyReadModel.CanStartGame.Should().BeTrue();
        lobbyReadModel.RequiredMinPlayers.Should().Be(3);
        lobbyReadModel.Players.Select(x => x.Username).Should().BeEquivalentTo(new[] { "thanos", "hulk", "ironman" });
        lobbyReadModel.GameStatus.Should().Be(GameStatus.NotStarted.Value);
    }
}
