using System.Net;
using Guexit.Game.Component.IntegrationTests.Builders;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.GameRoomAggregate.Events;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.Tests.Common;
using Microsoft.Extensions.DependencyInjection;

namespace Guexit.Game.Component.IntegrationTests;

public sealed class WhenReceivingGameStarted : ComponentTest
{
    public WhenReceivingGameStarted(GameWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task AssignsDeckToGameRoom()
    {
        var playerId1 = new PlayerId("1");
        var playerId2 = new PlayerId("2");
        var playerId3 = new PlayerId("3");
        var gameRoomId = new GameRoomId(Guid.NewGuid());

        await Save(new PlayerBuilder().WithId(playerId1).Build(),
            new PlayerBuilder().WithId(playerId2).Build(),
            new PlayerBuilder().WithId(playerId3).Build());

        await Save(new GameRoomBuilder()
            .WithId(gameRoomId)
            .WithCreator(playerId1)
            .WithPlayersThatJoined(playerId2, playerId3)
            .Build());

        var imageBuilder = new ImageBuilder();
        await Save(Enumerable.Range(0, 200)
            .Select(i => imageBuilder.WithId(Guid.NewGuid()).WithUrl(new Uri($"https://pablocompany/image/{i}")).Build())
            .ToArray());

        await StartGame(gameRoomId, playerId1);
        await ConsumeMessage(new GameStarted(gameRoomId));

        var gameRoom = await WebApplicationFactory.Services.CreateScope().ServiceProvider
            .GetRequiredService<IGameRoomRepository>().GetBy(gameRoomId);
        gameRoom!.Status.Should().Be(GameStatus.InProgress);
        gameRoom.Deck.Should().AllSatisfy(x => x.Url.ToString().StartsWith("https://pablocompany/image/"));
        gameRoom.PlayerHands.First(x => x.PlayerId == playerId1).Cards.Should().HaveCount(4);
    }

    private async Task StartGame(GameRoomId gameRoomId, PlayerId playerId1)
    {
        using var client = WebApplicationFactory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Post, $"game-rooms/{gameRoomId.Value}/start");
        request.AddPlayerIdHeader(playerId1);
        var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK, because: await response.Content.ReadAsStringAsync());
    }

}