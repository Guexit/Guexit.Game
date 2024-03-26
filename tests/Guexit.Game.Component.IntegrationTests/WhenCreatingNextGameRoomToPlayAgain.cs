using System.Net.Http.Json;
using Guexit.Game.Component.IntegrationTests.Extensions;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.Tests.Common;
using Guexit.Game.Tests.Common.Builders;
using Guexit.Game.Tests.Common.ObjectMothers;
using Guexit.Game.WebApi.Contracts.Responses;
using Microsoft.Extensions.DependencyInjection;

namespace Guexit.Game.Component.IntegrationTests;

public sealed class WhenCreatingNextGameRoomToPlayAgain : ComponentTest
{
    public WhenCreatingNextGameRoomToPlayAgain(GameWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task CreatesNextGameRoomAndLinksItToFinishedGameRoom()
    {
        var finishedGameRoomId = new GameRoomId(Guid.NewGuid());
        var nextGameRoomId = new GameRoomId(Guid.NewGuid());
        var playerId = new PlayerId("player1");
        await SaveInRepository(
            new PlayerBuilder().WithId(playerId).Build(),
            new PlayerBuilder().WithId("player2").Build(),
            new PlayerBuilder().WithId("player3").Build());
        await SaveInRepository(GameRoomObjectMother.Finished(finishedGameRoomId, playerId, ["player2", "player3"]));

        await using var scope = WebApplicationFactory.Services.CreateAsyncScope();
        var fakeGuidProvider = scope.ServiceProvider.GetRequiredService<FakeGuidProvider>();
        fakeGuidProvider.Returns(nextGameRoomId);
        
        using var response = await Send(HttpMethod.Post, $"/game-rooms/{finishedGameRoomId.Value}/create-next", playerId);

        await response.ShouldHaveSuccessStatusCode();

        var responseContent = await response.Content.ReadFromJsonAsync<CreateNextGameRoomResponse>();
        responseContent.Should().NotBeNull();
        responseContent!.NextGameRoomId.Should().Be(nextGameRoomId);

        var gameRoomRepository = scope.ServiceProvider.GetRequiredService<IGameRoomRepository>();
        var finishedGameRoom = await gameRoomRepository.GetBy(finishedGameRoomId);
        finishedGameRoom.Should().NotBeNull();
        finishedGameRoom!.NextGameRoomId.Should().Be(nextGameRoomId);
    }
}