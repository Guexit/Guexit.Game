using System.Net;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TryGuessIt.Game.Component.IntegrationTests.Builders;
using TryGuessIt.Game.Persistence;

namespace TryGuessIt.Game.Component.IntegrationTests;

public sealed class WhenJoiningGameRoom : ComponentTestBase
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public WhenJoiningGameRoom(GameWebApplicationFactory factory)
        : base(factory)
    {
        _serviceScopeFactory = factory.Services.GetRequiredService<IServiceScopeFactory>();
    }

    [Fact]
    public async Task PlayerIsAddedToGame()
    {
        var creatorId = new PlayerId("player1");
        var playerJoiningId = new PlayerId("player2");
        var gameRoomId = new GameRoomId(Guid.NewGuid());
        await AssumePlayer(playerJoiningId);
        await AssumeGameRoom(new GameRoom(gameRoomId, creatorId, new DateTimeOffset(2023, 1, 1, 2, 3, 4, TimeSpan.Zero)));

        using var client = WebApplicationFactory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Post, $"game-rooms/{gameRoomId.Value}/join");
        request.AddPlayerIdHeader(playerJoiningId);

        var response = await client.SendAsync(request);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK, await response.Content.ReadAsStringAsync());

        await AssertGameRoomHasPlayers(creatorId, playerJoiningId);
    }

    private async Task AssertGameRoomHasPlayers(PlayerId creator, PlayerId playerJoining)
    {
        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<GameDbContext>();
        var gameRooms = await dbContext.GameRooms.ToArrayAsync();

        gameRooms.Should().HaveCount(1);
        gameRooms[0].PlayerIds.Should().BeEquivalentTo(new[] { creator, playerJoining });
        gameRooms[0].RequiredMinPlayers.Should().Be(RequiredMinPlayers.Default);
    }

    private async Task AssumePlayer(PlayerId playerId)
    {
        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<GameDbContext>();

        dbContext.Add(new Player(playerId, "Juan Cuesta"));
        await dbContext.SaveChangesAsync();
    }

    private async Task AssumeGameRoom(GameRoom gameRoom)
    {
        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<GameDbContext>();

        dbContext.Add(gameRoom);
        await dbContext.SaveChangesAsync();
    }
}
