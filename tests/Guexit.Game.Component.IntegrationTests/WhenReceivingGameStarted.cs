using System.Net;
using Guexit.Game.Component.IntegrationTests.Builders;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.GameRoomAggregate.Events;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.Tests.Common;

namespace Guexit.Game.Component.IntegrationTests;

public sealed class WhenReceivingGameStarted : ComponentTestBase
{
    [Fact]
    public async Task AssignsDeckToGameRoom()
    {
        var playerId1 = new PlayerId("1");
        var playerId2 = new PlayerId("2");
        var playerId3 = new PlayerId("3");
        var gameRoomId = new GameRoomId(Guid.NewGuid());
        await Save(
            new PlayerBuilder().WithId(playerId1).Build(),
            new PlayerBuilder().WithId(playerId2).Build(),
            new PlayerBuilder().WithId(playerId3).Build());
        await Save(new GameRoomBuilder()
            .WithId(gameRoomId)
            .WithCreator(playerId1)
            .WithPlayersThatJoined(playerId2, playerId3)
            .Build());
        var imageBuilder = new ImageBuilder().WithLogicalShard(1);
        await Save(Enumerable.Range(0, 70)
            .Select(i => imageBuilder.WithId(Guid.NewGuid()).WithUrl(new Uri($"https://pablocompany/image/{i}")).Build())
            .ToArray());

        await StartGame(gameRoomId, playerId1);
        await ConsumeMessage(new GameStarted(gameRoomId));

        var gameRoom = GetSingle<GameRoom>(x => x.Id == gameRoomId);
        gameRoom.Status.Should().Be(GameStatus.InProgress);
    }

    private async Task StartGame(GameRoomId gameRoomId, PlayerId playerId1)
    {
        using var client = WebApplicationFactory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Post, $"game-rooms/{gameRoomId.Value}/start");
        request.AddPlayerIdHeader(playerId1);
        var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK, because: await response.Content.ReadAsStringAsync());
    }

    public WhenReceivingGameStarted(GameWebApplicationFactory factory) : base(factory)
    {
    }
}