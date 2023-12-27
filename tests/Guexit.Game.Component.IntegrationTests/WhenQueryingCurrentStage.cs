using System.Net.Http.Json;
using Guexit.Game.Component.IntegrationTests.Extensions;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.ReadModels.ReadModels;
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
        var gameRoom = new GameRoomBuilder().WithId(GameRoomId).WithCreator(creatorId).Build();

        await Save(gameRoom);
        
        using var response = await Send(HttpMethod.Get, $"/game-rooms/{GameRoomId.Value}/stages/current", creatorId);

        await response.ShouldHaveSuccessStatusCode();
        var readModel = await response.Content.ReadFromJsonAsync<GameStageReadModel>();
        readModel.Should().NotBeNull();
        readModel!.CurrentStage.Should().Be(GameStage.Lobby.Value);
    }

    [Fact]
    public async Task ReturnsBoardIfGameRoomIsInProgressAndNotAllPlayersHaveSubmittedCard()
    {
        var creatorId = new PlayerId("unai");
        var gameRoom = GameRoomBuilder.CreateStarted(GameRoomId, creatorId, ["poysky", "pablo"]).Build();

        await Save(gameRoom);

        using var response = await Send(HttpMethod.Get, $"/game-rooms/{GameRoomId.Value}/stages/current", creatorId);

        await response.ShouldHaveSuccessStatusCode();
        var readModel = await response.Content.ReadFromJsonAsync<GameStageReadModel>();
        readModel.Should().NotBeNull();
        readModel!.CurrentStage.Should().Be(GameStage.Board.Value);
    }

    [Fact]
    public async Task ReturnsVotingIfGameRoomIsInProgressAndEveryPlayerHaveSubmittedACard()
    {
        var creatorId = new PlayerId("unai");
        var gameRoom = GameRoomBuilder.CreateStarted(GameRoomId, creatorId, ["poysky", "pablo"])
            .WithStoryTellerStory("Pickle rick")
            .WithGuessingPlayerThatSubmittedCard("poysky", "pablo")
            .Build();

        await Save(gameRoom);

        using var response = await Send(HttpMethod.Get, $"/game-rooms/{GameRoomId.Value}/stages/current", creatorId);

        await response.ShouldHaveSuccessStatusCode();
        var readModel = await response.Content.ReadFromJsonAsync<GameStageReadModel>();
        readModel.Should().NotBeNull();
        readModel!.CurrentStage.Should().Be(GameStage.Voting.Value);
    }

    [Fact]
    public async Task ReturnsEndIfGameRoomIsHasFinished()
    {
        var creatorId = new PlayerId("unai");
        var gameRoom = GameRoomBuilder.CreateFinished(GameRoomId, creatorId, ["poysky", "pablo"]).Build();

        await Save(gameRoom);

        using var response = await Send(HttpMethod.Get, $"/game-rooms/{GameRoomId.Value}/stages/current", creatorId);

        await response.ShouldHaveSuccessStatusCode();
        var readModel = await response.Content.ReadFromJsonAsync<GameStageReadModel>();
        readModel.Should().NotBeNull();
        readModel!.CurrentStage.Should().Be(GameStage.End.Value);
    }
}