using System.Net;
using Guexit.Game.Component.IntegrationTests.Builders;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.Tests.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TryGuessIt.Game.Persistence;

namespace Guexit.Game.Component.IntegrationTests;

public sealed class WhenStartingGame : ComponentTestBase
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public WhenStartingGame(GameWebApplicationFactory factory) : base(factory)
    {
        _serviceScopeFactory = WebApplicationFactory.Services.GetRequiredService<IServiceScopeFactory>();
    }

    [Fact]
    public async Task IsStartedAndInitiatesCardsAssignation()
    {
        var gameRoomId = new GameRoomId(Guid.NewGuid());
        var playerId = new PlayerId("1");
        await AssumeExistingGameRoom(new GameRoomBuilder()
            .WithId(gameRoomId)
            .WithCreator(playerId).WithPlayersThatJoined("2", "3", "4")
            .WithMinRequiredPlayers(3)
            .Build());

        using var client = WebApplicationFactory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Post, $"game-rooms/{gameRoomId.Value}/start");
        request.AddPlayerIdHeader(playerId);
        var response = await client.SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await AssertGameRoomHasStarted(gameRoomId);
    }

    private async Task AssertGameRoomHasStarted(GameRoomId gameRoomId)
    {
        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<GameDbContext>();

        var gameRoom = await dbContext.GameRooms.SingleAsync(x => x.Id == gameRoomId);
        gameRoom.Should().NotBeNull();
        gameRoom!.Status.Should().Be(GameStatus.AssigningCards);
    }
}
