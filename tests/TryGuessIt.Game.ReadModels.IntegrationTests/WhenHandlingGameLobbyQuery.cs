using Microsoft.Extensions.Logging;
using NSubstitute;
using TryGuessIt.Game.Domain.Model.GameRoomAggregate;
using TryGuessIt.Game.Domain.Model.PlayerAggregate;
using TryGuessIt.Game.ReadModels.Queries;
using TryGuessIt.Game.ReadModels.QueryHandlers;
using TryGuessIt.Game.ReadModels.ReadModels;

namespace TryGuessIt.Game.ReadModels.IntegrationTests;

public class WhenHandlingGameLobbyQuery : ReadModelsIntegrationTestBase
{
    public WhenHandlingGameLobbyQuery(IntegrationTestFixture fixture) 
        : base(fixture)
    {
    }

    [Fact]
    public async Task GameLobbyReadModelIsReturnedAsync()
    {
        var gameRoomId = new GameRoomId(Guid.NewGuid());
        await DbContext.Players.AddAsync(new Player(new PlayerId("3"), "Emiliano"));
        await DbContext.GameRooms.AddAsync(
            new GameRoom(gameRoomId, new PlayerId("3"), new DateTimeOffset(2022, 1, 2, 3, 4, 5, TimeSpan.Zero)));
        await DbContext.SaveChangesAsync();
        DbContext.ChangeTracker.Clear();

        var handler = new GameLobbyQueryHandler(DbContext, Substitute.For<ILogger<QueryHandler<GameLobbyQuery, GameLobbyReadModel>>>());
        var readModel = await handler.Handle(new GameLobbyQuery(gameRoomId));

        readModel.Should().NotBeNull();
        readModel.RequiredMinPlayers.Should().Be(RequiredMinPlayers.Default.Count);
        readModel.Players.Should().BeEquivalentTo(new[] {new GameLobbyPlayerDto("Emiliano")});
    }
}
