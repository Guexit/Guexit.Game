using System.Net;
using Guexit.Game.Component.IntegrationTests.Builders;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.Persistence;
using Guexit.Game.Tests.Common;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Guexit.Game.Component.IntegrationTests;

public sealed class WhenCreatingGameRoom : ComponentTestBase
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
        
        var request = new HttpRequestMessage(HttpMethod.Post, $"game-rooms/{gameRoomId}");
        request.AddPlayerIdHeader(playerId);
        using var client = WebApplicationFactory.CreateClient();
        var response = await client.SendAsync(request);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK, await response.Content.ReadAsStringAsync());

        await AssertGameRoomWasCreated(gameRoomId, playerId);
    }

    [Fact]
    public async Task ReturnsNotFoundIfPlayerDoesNotExist()
    {
        var nonExistingPlayerId = new PlayerId("nonExistingPlayer");

        using var client = WebApplicationFactory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Post, "game-rooms");
        request.AddPlayerIdHeader(nonExistingPlayerId);
        var response = await client.SendAsync(request);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.NotFound, await response.Content.ReadAsStringAsync());
    }

    private async Task AssertGameRoomWasCreated(string gameRoomId, PlayerId playerId)
    {
        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<GameDbContext>();
        var gameRooms = await dbContext.GameRooms.ToArrayAsync();

        gameRooms.Should().HaveCount(1);
        gameRooms[0].PlayerIds.Single().Should().Be(playerId);
        gameRooms[0].RequiredMinPlayers.Should().Be(RequiredMinPlayers.Default);
    }
}
