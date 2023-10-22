using Guexit.Game.Component.IntegrationTests.Builders;
using Guexit.Game.Component.IntegrationTests.Extensions;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.Tests.Common;

namespace Guexit.Game.Component.IntegrationTests;

public sealed class WhenQueryingCurrentStage : ComponentTest
{
    private static readonly GameRoomId GameRoomId = new(Guid.Parse("fac3fd45-0ce9-4949-8af0-c31a1dd0210f"));
    
    public WhenQueryingCurrentStage(GameWebApplicationFactory factory) : base(factory)
    { }

    [Fact]
    public async Task ReturnsLobbyIfGameRoomHasNotStartedYet()
    {
        var creatorId = new PlayerId("gameRoomCreator");
        await Save(new GameRoomBuilder()
            .WithCreator(creatorId)
            .Build());
        
        using var client = WebApplicationFactory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Get, $"/game-rooms/{GameRoomId.Value}/board");
        request.AddPlayerIdHeader(creatorId);
        var response = await client.SendAsync(request);

        Assert.Fail("Work in progress");
        await response.ShouldHaveSuccessStatusCode();
    }
}