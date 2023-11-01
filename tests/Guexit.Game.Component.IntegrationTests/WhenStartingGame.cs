using Guexit.Game.Component.IntegrationTests.Extensions;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.Tests.Common;
using Microsoft.Extensions.DependencyInjection;

namespace Guexit.Game.Component.IntegrationTests;

public sealed class WhenStartingGame : ComponentTest
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public WhenStartingGame(GameWebApplicationFactory factory) : base(factory)
    {
        _serviceScopeFactory = WebApplicationFactory.Services.GetRequiredService<IServiceScopeFactory>();
    }

    [Fact]
    public async Task AssignsDeckAndStartsFirstRound()
    {
        var gameRoomId = new GameRoomId(Guid.NewGuid());
        var playerId1 = new PlayerId("1");
        var playerId2 = new PlayerId("2");
        var playerId3 = new PlayerId("3");
        await Save(new GameRoomBuilder()
            .WithId(gameRoomId)
            .WithCreator(playerId1).WithPlayersThatJoined(playerId2, playerId3)
            .Build());
        await Save(new PlayerBuilder().WithId(playerId1).Build(),
            new PlayerBuilder().WithId(playerId2).Build(),
            new PlayerBuilder().WithId(playerId3).Build());
        var images = Enumerable.Range(0, 200)
            .Select(_ => ImageBuilder.CreateValid().Build())
            .ToArray();
        await Save(images);
        
        using var response = await Send(HttpMethod.Post, $"game-rooms/{gameRoomId.Value}/start", playerId1);

        await response.ShouldHaveSuccessStatusCode();
        
        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        var gameRoomRepository = scope.ServiceProvider.GetRequiredService<IGameRoomRepository>();

        var gameRoom = await gameRoomRepository.GetBy(gameRoomId);
        gameRoom.Should().NotBeNull();
        gameRoom!.Status.Should().Be(GameStatus.InProgress);
        gameRoom.Deck.Should().NotBeEmpty()
            .And.Subject.Select(x => x.Url).Should().BeSubsetOf(images.Select(x => x.Url));
        gameRoom.PlayerHands.Should().AllSatisfy(x =>
        {
            x.Cards.Should().HaveCount(GameRoom.PlayerHandSize);
            x.Cards.Select(j => j.Url).Should().BeSubsetOf(images.Select(j => j.Url));
        });
    }
}
