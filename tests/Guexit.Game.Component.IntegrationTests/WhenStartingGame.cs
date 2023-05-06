using System.Net;
using Guexit.Game.Component.IntegrationTests.Builders;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.Persistence;
using Guexit.Game.Tests.Common;
using Microsoft.EntityFrameworkCore;
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
    public async Task IsStartedAndInitiatesCardsAssignation()
    {
        var gameRoomId = new GameRoomId(Guid.NewGuid());
        var playerId = new PlayerId("1");
        await Save(new GameRoomBuilder()
            .WithId(gameRoomId)
            .WithCreator(playerId).WithPlayersThatJoined("2", "3", "4")
            .WithMinRequiredPlayers(3)
            .Build());

        using var client = WebApplicationFactory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Post, $"game-rooms/{gameRoomId.Value}/start");
        request.AddPlayerIdHeader(playerId);
        var response = await client.SendAsync(request);
        
        response.StatusCode.Should().Be(HttpStatusCode.OK, because: await response.Content.ReadAsStringAsync());
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
