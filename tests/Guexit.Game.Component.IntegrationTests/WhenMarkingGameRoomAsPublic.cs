using Guexit.Game.Component.IntegrationTests.Extensions;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.Tests.Common.Builders;
using Microsoft.Extensions.DependencyInjection;

namespace Guexit.Game.Component.IntegrationTests;

public sealed class WhenMarkingGameRoomAsPublic : ComponentTest
{
    private static readonly GameRoomId GameRoomId = new GameRoomId(Guid.NewGuid());
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public WhenMarkingGameRoomAsPublic(GameWebApplicationFactory factory) : base(factory)
    {
        _serviceScopeFactory = WebApplicationFactory.Services.GetRequiredService<IServiceScopeFactory>();
    }

    [Fact]
    public async Task GameRoomIsMarkedAsPublic()
    {
        var creatorPlayerId = new PlayerId("player1");
        await SaveInRepository(new GameRoomBuilder()
            .WithId(GameRoomId)
            .WithCreator(creatorPlayerId)
            .WithPlayersThatJoined(["player2", "player3"])
            .WithIsPublic(false)
            .Build());

        using var response = await Send(HttpMethod.Post, $"/game-rooms/{GameRoomId.Value}/mark-as-public", creatorPlayerId.Value);
        await response.ShouldHaveSuccessStatusCode();

        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        var gameRoomRepository = scope.ServiceProvider.GetRequiredService<IGameRoomRepository>();

        var gameRoom = await gameRoomRepository.GetBy(GameRoomId);
        gameRoom.Should().NotBeNull();
        gameRoom!.IsPublic.Should().BeTrue();
    }
}
