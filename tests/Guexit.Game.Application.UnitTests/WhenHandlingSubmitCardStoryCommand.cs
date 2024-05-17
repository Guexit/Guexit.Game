using Guexit.Game.Application.CommandHandlers;
using Guexit.Game.Application.Commands;
using Guexit.Game.Application.Exceptions;
using Guexit.Game.Domain.Exceptions;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.GameRoomAggregate.Events;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.Tests.Common.Builders;

namespace Guexit.Game.Application.UnitTests;

public sealed class WhenHandlingSubmitCardStoryCommand
{
    private readonly IGameRoomRepository _gameRoomRepository;
    private readonly SubmitStoryTellerCardStoryCommandHandler _commandHandler;

    public WhenHandlingSubmitCardStoryCommand()
    {
        _gameRoomRepository = new FakeInMemoryGameRoomRepository();
        _commandHandler = new SubmitStoryTellerCardStoryCommandHandler(_gameRoomRepository);
    }

    [Fact]
    public async Task CardWithStoryIsSubmitted()
    {
        var gameRoomId = new GameRoomId(Guid.NewGuid());
        var storyTellerId = new PlayerId("storyTellerId");
        var story = "La tipica adolescente abuela";

        var gameRoom = GameRoomBuilder.CreateStarted(gameRoomId, storyTellerId, ["player2", "player3"]).Build();
        var selectedCard = gameRoom.PlayerHands.First(x => x.PlayerId == storyTellerId).Cards.First();
        await _gameRoomRepository.Add(gameRoom);

        await _commandHandler.Handle(new SubmitStoryTellerCardStoryCommand(storyTellerId, gameRoomId, selectedCard.Id.Value, story));

        gameRoom.CurrentStoryTeller.PlayerId.Should().BeEquivalentTo(storyTellerId);
        gameRoom.CurrentStoryTeller.Story.Should().BeEquivalentTo(story);
        gameRoom.SubmittedCards.Should().HaveCount(1);
        gameRoom.SubmittedCards.Single().PlayerId.Should().Be(storyTellerId);
        gameRoom.SubmittedCards.Single().Card.Should().Be(selectedCard);
        gameRoom.SubmittedCards.Single().GameRoomId.Should().Be(gameRoomId);
        gameRoom.DomainEvents.OfType<StoryTellerCardStorySubmitted>().Single().Should()
            .BeEquivalentTo(new StoryTellerCardStorySubmitted(gameRoomId, storyTellerId, selectedCard.Id, story));
    }

    [Fact]
    public async Task ThrowsGameRoomNotFoundExceptionIfDoesNotExist()
    {
        var nonExistingGameRoomId = new GameRoomId(Guid.NewGuid());

        var action = async () => await _commandHandler.Handle(
            new SubmitStoryTellerCardStoryCommand("anyPlayerId", nonExistingGameRoomId, Guid.NewGuid(), "anyStory"));

        await action.Should().ThrowAsync<GameRoomNotFoundException>();
    }

    [Fact]
    public async Task ThrowsCannotSubmitCardStoryIfGameRoomIsNotInProgressException()
    {
        var gameRoomId = new GameRoomId(Guid.NewGuid());
        var playerId = new PlayerId("creatorId");
        var anyCardId = new CardId(Guid.NewGuid());
        await _gameRoomRepository.Add(new GameRoomBuilder()
            .WithId(gameRoomId)
            .WithCreator(playerId)
            .WithPlayersThatJoined(new PlayerId("player1"), new PlayerId("player2"))
            .Build());

        var action = async () => 
            await _commandHandler.Handle(new SubmitStoryTellerCardStoryCommand(playerId, gameRoomId, anyCardId, "anyStory"));

        await action.Should().ThrowAsync<InvalidOperationForInProgressGame>();
    }

    [Fact]
    public async Task ThrowsCannotSubmitCardStoryIfPlayerIsNotCurrentStoryTellerException()
    {
        var gameRoomId = new GameRoomId(Guid.NewGuid());
        var nonStoryTellerId = new PlayerId("nonStoryTellerId");
        var anyCardId = Guid.NewGuid();
        await _gameRoomRepository.Add(
            GameRoomBuilder.CreateStarted(gameRoomId, "player1", ["player2", nonStoryTellerId])
            .Build());

        var action = async () => 
            await _commandHandler.Handle(new SubmitStoryTellerCardStoryCommand(nonStoryTellerId, gameRoomId, anyCardId, "anyStory"));

        await action.Should().ThrowAsync<InvalidCardStorySubmissionForNonStoryTellerException>();
    }

    [Fact]
    public async Task ThrowsCardStoryAlreadySubmittedException()
    {
        var gameRoomId = new GameRoomId(Guid.NewGuid());
        var storyTellerId = new PlayerId("storyTellerId");
        var story = "La tipica adolescente abuela";
        var gameRoom = GameRoomBuilder.CreateStarted(gameRoomId, storyTellerId, [new PlayerId("player2"), new PlayerId("player3")
        ]).Build();
        var selectedCardId = gameRoom.PlayerHands.First(x => x.PlayerId == storyTellerId).Cards.First().Id.Value;
        await _gameRoomRepository.Add(gameRoom);

        await _commandHandler.Handle(new SubmitStoryTellerCardStoryCommand(storyTellerId, gameRoomId, selectedCardId, story));
        var action = async () 
            => await _commandHandler.Handle(new SubmitStoryTellerCardStoryCommand(storyTellerId, gameRoomId, selectedCardId, story));

        await action.Should().ThrowAsync<StoryAlreadySubmittedException>();
    }

    [Fact]
    public async Task ThrowsCardNotFoundInPlayerHandException()
    {
        var gameRoomId = new GameRoomId(Guid.NewGuid());
        var storyTellerId = new PlayerId("storyTellerId");
        var story = "La tipica adolescente abuela";
        var gameRoom = GameRoomBuilder.CreateStarted(gameRoomId, storyTellerId, ["player2", "player3"]).Build();
        var nonExistingCardId = Guid.NewGuid();
        await _gameRoomRepository.Add(gameRoom);

        var action = async () 
            => await _commandHandler.Handle(new SubmitStoryTellerCardStoryCommand(storyTellerId, gameRoomId, nonExistingCardId, story));

        await action.Should().ThrowAsync<CardNotFoundInPlayerHandException>();
    } 
    
    [Fact]
    public async Task ThrowsEmptyCardStoryException()
    {
        var gameRoomId = new GameRoomId(Guid.NewGuid());
        var storyTellerId = new PlayerId("storyTellerId");
        var emptyStory = string.Empty;
        var gameRoom = GameRoomBuilder.CreateStarted(gameRoomId, storyTellerId, ["player2", "player3"]).Build();
        var selectedCardId = gameRoom.PlayerHands.First(x => x.PlayerId == storyTellerId).Cards.First().Id.Value;
        await _gameRoomRepository.Add(gameRoom);

        var action = async () 
            => await _commandHandler.Handle(new SubmitStoryTellerCardStoryCommand(storyTellerId, gameRoomId, selectedCardId, emptyStory));

        await action.Should().ThrowAsync<EmptyCardStoryException>();
    }
}
