using Guexit.Game.Component.IntegrationTests.Extensions;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Tests.Common.Builders;
using Microsoft.Extensions.DependencyInjection;

namespace Guexit.Game.Component.IntegrationTests;

public sealed class WhenReservingCardsForReRoll : ComponentTest
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public WhenReservingCardsForReRoll(GameWebApplicationFactory factory) : base(factory)
    {
        _serviceScopeFactory = WebApplicationFactory.Services.GetRequiredService<IServiceScopeFactory>();
    }

    [Fact]
    public async Task CardsAreReservedForReRoll()
    {
        var gameRoomId = new GameRoomId(Guid.NewGuid());
        var reRollingPlayer = new PlayerBuilder().WithId("manu").WithUsername("manuminga").Build();
        var player2 = new PlayerBuilder().WithId("unai").WithUsername("unaigay").Build();
        var player3 = new PlayerBuilder().WithId("pablo").WithUsername("pablodiablo").Build();

        var expectedReservedImages = new[]
        {
            ImageBuilder.CreateValid().Build(),
            ImageBuilder.CreateValid().Build(),
            ImageBuilder.CreateValid().Build()
        };

        await SaveInRepository(expectedReservedImages);
        await SaveInRepository(GameRoomBuilder.CreateStarted(gameRoomId, reRollingPlayer.Id, [player2.Id, player3.Id]).Build());
        await SaveInRepository(reRollingPlayer, player2, player3);

        using var response = await Send(
            HttpMethod.Post, 
            $"/game-rooms/{gameRoomId.Value}/reserve-cards-for-re-roll", 
            reRollingPlayer.Id
        );
        await response.ShouldHaveSuccessStatusCode();

        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        var gameRoomRepository = scope.ServiceProvider.GetRequiredService<IGameRoomRepository>();

        var gameRoom = await gameRoomRepository.GetBy(gameRoomId);
        gameRoom.Should().NotBeNull();
        gameRoom!.CurrentCardReRolls.Should().HaveCount(1);
        
        var cardReRoll = gameRoom.CurrentCardReRolls.Single();
        cardReRoll.PlayerId.Should().Be(reRollingPlayer.Id);
        cardReRoll.Status.Should().Be(CardReRollStatus.InProgress);
        cardReRoll.ReservedCards.Select(x => x.Url).Should().BeEquivalentTo(expectedReservedImages.Select(x => x.Url));
    }
}
