﻿using Guexit.Game.Application.CommandHandlers;
using Guexit.Game.Application.Commands;
using Guexit.Game.Application.Exceptions;
using Guexit.Game.Domain.Exceptions;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.GameRoomAggregate.Events;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.Tests.Common;

namespace Guexit.Game.Application.UnitTests;

public sealed class WhenHandlingSubmitCardStoryCommand
{
    private readonly IGameRoomRepository _gameRoomRepository;
    private readonly SubmitCardStoryCommandHandler _commandHandler;

    public WhenHandlingSubmitCardStoryCommand()
    {
        _gameRoomRepository = new FakeInMemoryGameRoomRepository();
        _commandHandler = new SubmitCardStoryCommandHandler(Substitute.For<IUnitOfWork>(), _gameRoomRepository);
    }

    [Fact]
    public async Task CardWithStoryIsSubmitted()
    {
        var gameRoomId = new GameRoomId(Guid.NewGuid());
        var storyTellerId = new PlayerId("nonStoryTellerId");
        var story = "La tipica adolescente abuela";
        var gameRoom = GameRoomBuilder.CreateStarted(gameRoomId, storyTellerId, new[] { new PlayerId("player2"), new PlayerId("player3") }).Build();
        var selectedCardId = gameRoom.PlayerHands.First(x => x.PlayerId == storyTellerId).Cards.First().Id;
        await _gameRoomRepository.Add(gameRoom);

        await _commandHandler.Handle(new SubmitCardStoryCommand(storyTellerId, gameRoomId, selectedCardId, story));

        gameRoom.CurrentStoryTeller.PlayerId.Should().BeEquivalentTo(storyTellerId);
        gameRoom.CurrentStoryTeller.SelectedCardId.Should().BeEquivalentTo(selectedCardId);
        gameRoom.CurrentStoryTeller.Story.Should().BeEquivalentTo(story);
        gameRoom.DomainEvents.OfType<CardStorySubmitted>().Single().Should()
            .BeEquivalentTo(new CardStorySubmitted(gameRoomId, storyTellerId, selectedCardId, story));
    }

    [Fact]
    public async Task ThrowsGameRoomNotFoundExceptionIfDoesNotExist()
    {
        var nonExistingGameRoomId = new GameRoomId(Guid.NewGuid());

        var action = async () => await _commandHandler.Handle(
            new SubmitCardStoryCommand("anyPlayerId", nonExistingGameRoomId, new CardId(Guid.NewGuid()), "anyStory"));

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
            await _commandHandler.Handle(new SubmitCardStoryCommand(playerId, gameRoomId, anyCardId, "anyStory"));

        await action.Should().ThrowAsync<CannotSubmitCardStoryIfGameRoomIsNotInProgressException>();
    }

    [Fact]
    public async Task ThrowsCannotSubmitCardStoryIfPlayerIsNotCurrentStoryTellerException()
    {
        var gameRoomId = new GameRoomId(Guid.NewGuid());
        var nonStoryTellerId = new PlayerId("nonStoryTellerId");
        var anyCardId = new CardId(Guid.NewGuid());
        await _gameRoomRepository.Add(
            GameRoomBuilder.CreateStarted(gameRoomId, new PlayerId("player1"), new[] { new PlayerId("player2"), nonStoryTellerId })
            .Build());

        var action = async () => 
            await _commandHandler.Handle(new SubmitCardStoryCommand(nonStoryTellerId, gameRoomId, anyCardId, "anyStory"));

        await action.Should().ThrowAsync<CannotSubmitCardStoryIfPlayerIsNotCurrentStoryTellerException>();
    }

    [Fact]
    public async Task ThrowsCardStoryAlreadySubmittedException()
    {
        var gameRoomId = new GameRoomId(Guid.NewGuid());
        var storyTellerId = new PlayerId("storyTellerId");
        var story = "La tipica adolescente abuela";
        var gameRoom = GameRoomBuilder.CreateStarted(gameRoomId, storyTellerId, new[] { new PlayerId("player2"), new PlayerId("player3") }).Build();
        var selectedCardId = gameRoom.PlayerHands.First(x => x.PlayerId == storyTellerId).Cards.First().Id;
        await _gameRoomRepository.Add(gameRoom);

        await _commandHandler.Handle(new SubmitCardStoryCommand(storyTellerId, gameRoomId, selectedCardId, story));
        var action = async () 
            => await _commandHandler.Handle(new SubmitCardStoryCommand(storyTellerId, gameRoomId, selectedCardId, story));

        await action.Should().ThrowAsync<CardStoryAlreadySubmittedException>();
    }

    [Fact]
    public async Task ThrowsCardNotFoundInPlayerHandException()
    {
        var gameRoomId = new GameRoomId(Guid.NewGuid());
        var storyTellerId = new PlayerId("storyTellerId");
        var story = "La tipica adolescente abuela";
        var gameRoom = GameRoomBuilder.CreateStarted(gameRoomId, storyTellerId, new[] { new PlayerId("player2"), new PlayerId("player3") }).Build();
        var nonExistingCardId = new CardId(Guid.NewGuid());
        await _gameRoomRepository.Add(gameRoom);

        var action = async () 
            => await _commandHandler.Handle(new SubmitCardStoryCommand(storyTellerId, gameRoomId, nonExistingCardId, story));

        await action.Should().ThrowAsync<CardNotFoundInPlayerHandException>();
    } 
    
    [Fact]
    public async Task ThrowsEmptyCardStoryException()
    {
        var gameRoomId = new GameRoomId(Guid.NewGuid());
        var storyTellerId = new PlayerId("storyTellerId");
        var emptyStory = string.Empty;
        var gameRoom = GameRoomBuilder.CreateStarted(gameRoomId, storyTellerId, new[] { new PlayerId("player2"), new PlayerId("player3") }).Build();
        var selectedCardId = gameRoom.PlayerHands.First(x => x.PlayerId == storyTellerId).Cards.First().Id;
        await _gameRoomRepository.Add(gameRoom);

        var action = async () 
            => await _commandHandler.Handle(new SubmitCardStoryCommand(storyTellerId, gameRoomId, selectedCardId, emptyStory));

        await action.Should().ThrowAsync<EmptyCardStoryException>();
    }
}
