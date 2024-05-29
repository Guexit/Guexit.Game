using System.Net;
using System.Net.Http.Json;
using Guexit.Game.Component.IntegrationTests.Extensions;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.ReadModels.ReadModels;
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
        var player3 = new PlayerBuilder().WithId("playerId3").WithUsername("groot@guexit.com").Build();
        var story = "El tipico adolescente abuelo";

        var gameRoom = GameRoomBuilder.CreateStarted(gameRoomId, player1.Id, [player2.Id, player3.Id])
            .WithStoryTellerStory(story)
            .WithGuessingPlayerThatSubmittedCard(player2.Id)
            .Build();
        await SaveInRepository(gameRoom);
        await SaveInRepository(player1, player2, player3);

        using var response = await Send(HttpMethod.Get, $"/game-rooms/{gameRoom.Id.Value}/board", player1.Id);

        await response.ShouldHaveSuccessStatusCode();

        var readModel = await response.Content.ReadFromJsonAsync<BoardReadModel>();

        readModel.Should().NotBeNull();
        
        readModel!.GameRoomId.Should().Be(gameRoomId);

        readModel.CurrentStoryTeller.PlayerId.Should().Be(player1.Id);
        readModel.CurrentStoryTeller.Story.Should().Be(story);
        readModel.CurrentStoryTeller.Username.Should().Be(player1.Username);
        readModel.CurrentStoryTeller.Nickname.Should().Be(player1.Nickname.Value);
        readModel.IsCurrentUserStoryTeller.Should().BeTrue();
        readModel.CurrentUserSubmittedCard.Should().NotBeNull();

        var expectedPlayerHand = gameRoom.PlayerHands.Single(x => x.PlayerId == player1.Id)
            .Cards.Select(x => new BoardReadModel.CardDto { Id = x.Id, Url = x.Url });
        readModel.PlayerHand.Should().BeEquivalentTo(expectedPlayerHand);

        readModel.CurrentPlayer.PlayerId.Should().Be(player1.Id.Value);
        readModel.CurrentPlayer.Nickname.Should().Be(player1.Nickname.Value);
        readModel.CurrentPlayer.Username.Should().Be(player1.Username);
        
        readModel.GuessingPlayers.Should().HaveCount(2);

        readModel.GuessingPlayers.First(x => x.PlayerId == player2.Id).Username.Should().Be(player2.Username);
        readModel.GuessingPlayers.First(x => x.PlayerId == player2.Id).Nickname.Should().Be(player2.Nickname.Value);
        readModel.GuessingPlayers.First(x => x.PlayerId == player2.Id).HasSubmittedCardAlready.Should().BeTrue();

        readModel.GuessingPlayers.First(x => x.PlayerId == player3.Id).Username.Should().Be(player3.Username);
        readModel.GuessingPlayers.First(x => x.PlayerId == player3.Id).Nickname.Should().Be(player3.Nickname.Value);
        readModel.GuessingPlayers.First(x => x.PlayerId == player3.Id).HasSubmittedCardAlready.Should().BeFalse();

        readModel.CurrentPlayerCardReRollState.Should().Be(CardReRollState.Empty.Value);
    }

    [Fact]
    public async Task ReturnsNotFoundIfGameRoomDoesNotExist()
    {
        var nonExistingGameRoomId = new GameRoomId(Guid.NewGuid());
        var playerId = new PlayerId("storyTellerId"); 
        await SaveInRepository(new PlayerBuilder().WithId(playerId).WithUsername("gamora").Build());

        using var response = await Send(HttpMethod.Get, $"/game-rooms/{nonExistingGameRoomId.Value}/board", playerId);

        await response.ShouldHaveStatusCode(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ReturnsIfCurrentUserIsNotTheCurrentStoryTeller()
    {
        var gameRoomId = new GameRoomId(Guid.NewGuid());
        var initialStoryTeller = new PlayerId("storyTellerId");
        var playerId2 = new PlayerId("player2");
        var playerId3 = new PlayerId("player3");
        var gameRoom = GameRoomBuilder.CreateStarted(gameRoomId, initialStoryTeller, [playerId2, playerId3]).Build();
        await SaveInRepository(gameRoom);
        await SaveInRepository(
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
        await SaveInRepository(gameRoom);
        await SaveInRepository(
        [
            new PlayerBuilder().WithId(playerId1).WithUsername("gamora").Build(),
            new PlayerBuilder().WithId(playerId2).WithUsername("starlord").Build(),
            new PlayerBuilder().WithId(playerId3).WithUsername("wroot").Build()
        ]);

        using var response = await Send(HttpMethod.Get, $"/game-rooms/{gameRoom.Id.Value}/board", playerId1);

        await response.ShouldHaveStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ReturnsCurrentReRollStateAvailableIfCardsWereReservedButItHasNotCompleted()
    {
        var gameRoomId = new GameRoomId(Guid.NewGuid());
        var player1 = new PlayerBuilder().WithId("storyTellerId").WithUsername("gamora@guexit.com").Build();
        var player2 = new PlayerBuilder().WithId("playerId2").WithUsername("starlord@guexit.com").Build();
        var player3 = new PlayerBuilder().WithId("playerId3").WithUsername("groot@guexit.com").Build();

        var gameRoom = GameRoomBuilder.CreateStarted(gameRoomId, player1.Id, [player2.Id, player3.Id])
            .WithPlayerThatReservedCardsForReRoll(player1.Id)
            .Build();

        await SaveInRepository(gameRoom);
        await SaveInRepository(player1, player2, player3);

        using var response = await Send(HttpMethod.Get, $"/game-rooms/{gameRoom.Id.Value}/board", player1.Id);

        await response.ShouldHaveSuccessStatusCode();

        var readModel = await response.Content.ReadFromJsonAsync<BoardReadModel>();

        readModel.Should().NotBeNull();
        readModel!.CurrentPlayerCardReRollState.Should().Be(CardReRollState.Available.Value);
    }

    [Fact]
    public async Task ReturnsCurrentReRollStateCompletedIfPlayerAlreadyReRolledACardThisRound()
    {
        var gameRoomId = new GameRoomId(Guid.NewGuid());
        var player1 = new PlayerBuilder().WithId("storyTellerId").WithUsername("gamora@guexit.com").Build();
        var player2 = new PlayerBuilder().WithId("playerId2").WithUsername("starlord@guexit.com").Build();
        var player3 = new PlayerBuilder().WithId("playerId3").WithUsername("groot@guexit.com").Build();

        var gameRoom = GameRoomBuilder.CreateStarted(gameRoomId, player1.Id, [player2.Id, player3.Id])
            .WithPlayerThatReservedCardsForReRoll(player1.Id, completed: true)
            .Build();

        await SaveInRepository(gameRoom);
        await SaveInRepository(player1, player2, player3);

        using var response = await Send(HttpMethod.Get, $"/game-rooms/{gameRoom.Id.Value}/board", player1.Id);

        await response.ShouldHaveSuccessStatusCode();

        var readModel = await response.Content.ReadFromJsonAsync<BoardReadModel>();

        readModel.Should().NotBeNull();
        readModel!.CurrentPlayerCardReRollState.Should().Be(CardReRollState.Completed.Value);
    }
}
