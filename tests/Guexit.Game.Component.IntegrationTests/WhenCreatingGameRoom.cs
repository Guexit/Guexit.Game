using System.Net;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.Persistence;
using Guexit.Game.Tests.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Guexit.Game.Component.IntegrationTests;

public sealed class WhenCreatingGameRoom : ComponentTest
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public WhenCreatingGameRoom(GameWebApplicationFactory factory) 
        : base(factory)
    {
        _serviceScopeFactory = factory.Services.GetRequiredService<IServiceScopeFactory>();
    }

    [Fact]
    public async Task GameRoomIsCreated()
    {
        var gameRoomId = Guid.NewGuid().ToString();
        var playerId = new PlayerId("player1");
        await Save(new PlayerBuilder().WithId(playerId).Build());
        
        using var response = await Send(HttpMethod.Post, $"game-rooms/{gameRoomId}", playerId);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK, await response.Content.ReadAsStringAsync());

        await AssertGameRoomWasCreated(gameRoomId, playerId);
    }

    [Fact]
    public async Task ReturnsNotFoundIfPlayerDoesNotExist()
    {
        var gameRoomId = Guid.NewGuid();
        var nonExistingPlayerId = new PlayerId("nonExistingPlayer");

        using var response = await Send(HttpMethod.Post, $"game-rooms/{gameRoomId}", authenticatedPlayerId: nonExistingPlayerId);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound, await response.Content.ReadAsStringAsync());
    }

    private async Task AssertGameRoomWasCreated(string gameRoomId, PlayerId playerId)
    {
        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<GameDbContext>();
        var gameRooms = await dbContext.GameRooms.ToArrayAsync();
        gameRooms.Should().HaveCount(1);

        var gameRoom = gameRooms.Single();
        gameRoom.CreatedBy.Should().Be(playerId);
        gameRoom.PlayerIds.Single().Should().Be(playerId);
        gameRoom.RequiredMinPlayers.Should().Be(RequiredMinPlayers.Default);
    }
}
