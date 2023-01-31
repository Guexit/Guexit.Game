using System.Net;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TryGuessIt.Game.Component.IntegrationTests.Builders;
using TryGuessIt.Game.Persistence;

namespace TryGuessIt.Game.Component.IntegrationTests;

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
        var playerId = new PlayerId("player1");
        await AssumePlayerWasCreated(playerId);
        
        var request = new HttpRequestMessage(HttpMethod.Post, "game-rooms");
        request.AddPlayerIdHeader(playerId);
        using var client = WebApplicationFactory.CreateClient();
        var response = await client.SendAsync(request);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK, await response.Content.ReadAsStringAsync());

        await AssertGameRoomWasCreated(playerId); 
    }

    private async Task AssumePlayerWasCreated(PlayerId playerId)
    {
        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<GameDbContext>();

        dbContext.Add(new Player(playerId, "Juan Cuesta"));
        await dbContext.SaveChangesAsync();
    }

    private async Task AssertGameRoomWasCreated(PlayerId playerId)
    {
        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<GameDbContext>();
        var gameRooms = await dbContext.GameRooms.ToArrayAsync();

        gameRooms.Should().HaveCount(1);
        gameRooms[0].PlayerIds.Single().Should().Be(playerId);
        gameRooms[0].RequiredMinPlayers.Should().Be(RequiredMinPlayers.Default);
    }
}
