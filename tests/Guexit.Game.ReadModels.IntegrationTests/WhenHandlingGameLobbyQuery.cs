using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.ReadModels.Queries;
using Guexit.Game.ReadModels.QueryHandlers;
using Guexit.Game.ReadModels.ReadModels;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace TryGuessIt.Game.ReadModels.IntegrationTests;

public class WhenHandlingGameLobbyQuery : ReadModelsIntegrationTestBase
{
    public WhenHandlingGameLobbyQuery(ReadModelsIntegrationTestFixture fixture) 
        : base(fixture)
    {
    }

    [Fact]
    public async Task ReturnsGameLobby()
    {
        var gameRoomId = Guid.NewGuid();
        await DbContext.Players.AddAsync(new Player(new PlayerId("3"), "Emiliano"));
        await DbContext.GameRooms.AddAsync(
            new GameRoom(new GameRoomId(gameRoomId), new PlayerId("3"), new DateTimeOffset(2022, 1, 2, 3, 4, 5, TimeSpan.Zero))
        );
        await DbContext.SaveChangesAsync();
        DbContext.ChangeTracker.Clear();

        var queryHandler = new GameLobbyQueryHandler(DbContext, Substitute.For<ILogger<GameLobbyQueryHandler>>());
        var lobbyReadModel = await queryHandler.Handle(new GameLobbyQuery(gameRoomId));

        lobbyReadModel.Should().NotBeNull();
        lobbyReadModel!.GameRoomId.Should().Be(gameRoomId);
        lobbyReadModel.RequiredMinPlayers.Should().Be(RequiredMinPlayers.Default.Count);
        lobbyReadModel.Players.Should().BeEquivalentTo(new[] {new GameLobbyPlayerDto("Emiliano")});
        lobbyReadModel.CanStartGame.Should().BeFalse();
    }
}
