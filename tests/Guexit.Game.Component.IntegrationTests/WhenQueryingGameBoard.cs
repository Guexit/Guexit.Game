using System.Net;
using System.Net.Http.Json;
using Guexit.Game.Component.IntegrationTests.Extensions;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.ReadModels.ReadModels;
using Guexit.Game.Tests.Common;
using Guexit.Game.Tests.Common.Builders;

namespace Guexit.Game.Component.IntegrationTests;

public sealed class WhenQueryingGameBoard : ComponentTest
{
    public WhenQueryingGameBoard(GameWebApplicationFactory factory) : base(factory)
    { }

    [Fact]
    public async Task ReturnsGameBoardReadModel()
    {
        var gameRoomId = new GameRoomId(Guid.NewGuid());
        var player1 = new PlayerBuilder().WithId("storyTellerId").WithUsername("gamora@guexit.com").Build(); 
        var player2 = new PlayerBuilder().WithId("playerId2").WithUsername("starlord@guexit.com").Build(); 
        var player3 = new PlayerBuilder().WithId("playerId3").WithUsername("wroot@guexit.com").Build();
        var story = "El tipico adolescente abuelo";

        var gameRoom = GameRoomBuilder.CreateStarted(gameRoomId, player1.Id, [player2.Id, player3.Id])
            .WithStoryTellerStory(story)
            .WithGuessingPlayerThatSubmittedCard(player2.Id)
            .Build();
        await Save(gameRoom);
        await Save(player1, player2, player3);

        using var response = await Send(HttpMethod.Get, $"/game-rooms/{gameRoom.Id.Value}/board", player1.Id);

        await response.ShouldHaveSuccessStatusCode();

        var responseContent = await response.Content.ReadFromJsonAsync<BoardReadModel>();

        responseContent.Should().NotBeNull();
        
        responseContent!.GameRoomId.Should().Be(gameRoomId);

        responseContent.CurrentStoryTeller.PlayerId.Should().Be(player1.Id);
        responseContent.CurrentStoryTeller.Story.Should().Be(story);
        responseContent.CurrentStoryTeller.Username.Should().Be(player1.Username);
        responseContent.CurrentStoryTeller.Nickname.Should().Be(player1.Nickname.Value);
        responseContent.IsCurrentUserStoryTeller.Should().BeTrue();
        responseContent.CurrentUserSubmittedCard.Should().NotBeNull();

        var expectedPlayerHand = gameRoom.PlayerHands.Single(x => x.PlayerId == player1.Id)
            .Cards.Select(x => new BoardReadModel.CardDto { Id = x.Id, Url = x.Url });
        responseContent.PlayerHand.Should().BeEquivalentTo(expectedPlayerHand);

        responseContent.GuessingPlayers.Should().HaveCount(2);

        responseContent.GuessingPlayers.First(x => x.PlayerId == player2.Id).Username.Should().Be(player2.Username);
        responseContent.GuessingPlayers.First(x => x.PlayerId == player2.Id).Nickname.Should().Be(player2.Nickname.Value);
        responseContent.GuessingPlayers.First(x => x.PlayerId == player2.Id).HasSubmittedCardAlready.Should().BeTrue();

        responseContent.GuessingPlayers.First(x => x.PlayerId == player3.Id).Username.Should().Be(player3.Username);
        responseContent.GuessingPlayers.First(x => x.PlayerId == player3.Id).Nickname.Should().Be(player3.Nickname.Value);
        responseContent.GuessingPlayers.First(x => x.PlayerId == player3.Id).HasSubmittedCardAlready.Should().BeFalse();
    }

    [Fact]
    public async Task ReturnsNotFoundIfGameRoomDoesNotExist()
    {
        var nonExistingGameRoomId = new GameRoomId(Guid.NewGuid());
        var playerId = new PlayerId("storyTellerId"); 
        await Save(new PlayerBuilder().WithId(playerId).WithUsername("gamora").Build());

        using var response = await Send(HttpMethod.Get, $"/game-rooms/{nonExistingGameRoomId.Value}/board", playerId);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound, because: await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task ReturnsIfCurrentUserIsNotTheCurrentStoryTeller()
    {
        var gameRoomId = new GameRoomId(Guid.NewGuid());
        var initialStoryTeller = new PlayerId("storyTellerId");
        var playerId2 = new PlayerId("player2");
        var playerId3 = new PlayerId("player3");
        var gameRoom = GameRoomBuilder.CreateStarted(gameRoomId, initialStoryTeller, [playerId2, playerId3]).Build();
        await Save(gameRoom);
        await Save(
        [
            new PlayerBuilder().WithId(initialStoryTeller).WithUsername("gamora").Build(),
            new PlayerBuilder().WithId(playerId2).WithUsername("starlord").Build(),
            new PlayerBuilder().WithId(playerId3).WithUsername("wroot").Build()
        ]);

        using var response = await Send(HttpMethod.Get, $"/game-rooms/{gameRoom.Id.Value}/board", playerId3);

        await response.ShouldHaveSuccessStatusCode();
        var responseContent = await response.Content.ReadFromJsonAsync<BoardReadModel>();
        responseContent.Should().NotBeNull();
        responseContent!.IsCurrentUserStoryTeller.Should().BeFalse();
    }

    [Fact]
    public async Task ReturnsBadRequestIfGameIsNotInProgress()
    {
        var gameRoomId = new GameRoomId(Guid.NewGuid());
        var playerId1 = new PlayerId("storyTellerId");
        var playerId2 = new PlayerId("player2");
        var playerId3 = new PlayerId("player3");
        var gameRoom = new GameRoomBuilder()
            .WithId(gameRoomId)
            .WithCreator(playerId1).WithPlayersThatJoined(playerId2, playerId3)
            .Build();
        await Save(gameRoom);
        await Save(
        [
            new PlayerBuilder().WithId(playerId1).WithUsername("gamora").Build(),
            new PlayerBuilder().WithId(playerId2).WithUsername("starlord").Build(),
            new PlayerBuilder().WithId(playerId3).WithUsername("wroot").Build()
        ]);

        using var response = await Send(HttpMethod.Get, $"/game-rooms/{gameRoom.Id.Value}/board", playerId1);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest, because: await response.Content.ReadAsStringAsync());
    }
}
