using Guexit.Game.Application.CommandHandlers;
using Guexit.Game.Application.Commands;
using Guexit.Game.Application.Exceptions;
using Guexit.Game.Domain.Exceptions;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.Tests.Common;

namespace Guexit.Game.Application.UnitTests;

public sealed class WhenHandlingVoteSubmittedCardCommand
{
    private static readonly GameRoomId GameRoomId = new(Guid.NewGuid());

    private readonly IGameRoomRepository _gameRoomRepository;
    private readonly VoteCardCommandHandler _commandHandler;
    
    public WhenHandlingVoteSubmittedCardCommand()
    {
        _gameRoomRepository = new FakeInMemoryGameRoomRepository();
        _commandHandler = new VoteCardCommandHandler(Substitute.For<IUnitOfWork>(), _gameRoomRepository);
    }

    [Fact]
    public async Task VotingPlayerIsAddedToSubmittedCardVoters()
    {
        var votingPlayerId = new PlayerId("votingPlayer");
        var gameRoom = GameRoomBuilder.CreateStarted(GameRoomId, "storyTellerId", new[] { votingPlayerId, new PlayerId("player3") })
            .WithStoryTellerStory("Any story")
            .WithGuessingPlayerThatSubmittedCard(votingPlayerId, "player3")
            .Build();
        var votedCardId = gameRoom.SubmittedCards.First(x => x.PlayerId != votingPlayerId).Card.Id;
        await _gameRoomRepository.Add(gameRoom);

        await _commandHandler.Handle(new VoteCardCommand(votingPlayerId, GameRoomId, votedCardId));
        
        var submittedCard = gameRoom.SubmittedCards.Single(x => x.Card.Id == votedCardId);
        submittedCard.Voters.Should().Contain(votingPlayerId);
    }
    
    [Fact]
    public async Task ThrowsGameRoomNotFoundExceptionIfDoesNotExist()
    {
        var nonExistingGameRoomId = new GameRoomId(Guid.NewGuid());

        var action = async () => await _commandHandler.Handle(
            new VoteCardCommand("anyPlayerId", nonExistingGameRoomId, new CardId(Guid.NewGuid())));

        await action.Should().ThrowAsync<GameRoomNotFoundException>();
    }

    [Fact]
    public async Task ThrowsCannotVoteCardIfGameRoomIsNotInProgressException()
    {
        var gameRoomId = new GameRoomId(Guid.NewGuid());
        var votingPlayerId = new PlayerId("creatorId");
        var anyCardId = new CardId(Guid.NewGuid());
        await _gameRoomRepository.Add(new GameRoomBuilder()
            .WithId(gameRoomId)
            .WithCreator(votingPlayerId)
            .WithPlayersThatJoined(new PlayerId("player1"), new PlayerId("player2"))
            .Build());

        var action = async () => 
            await _commandHandler.Handle(new VoteCardCommand(votingPlayerId, gameRoomId, anyCardId));

        await action.Should().ThrowAsync<CannotVoteCardIfGameRoomIsNotInProgressException>();
    }
    
    [Fact]
    public async Task ThrowsPlayerNotFoundInCurrentGuessingPlayersExceptionException()
    {
        var storyTellerId = new PlayerId("storyTellerId");
        var anyCardId = new CardId(Guid.NewGuid());
        await _gameRoomRepository.Add(GameRoomBuilder.CreateStarted(GameRoomId, storyTellerId, new PlayerId[] { "player2", "player3" })
            .WithStoryTellerStory("Any story")
            .WithGuessingPlayerThatSubmittedCard("player2", "player3")
            .Build());

        var action = async () =>
            await _commandHandler.Handle(new VoteCardCommand(storyTellerId, GameRoomId, anyCardId));

        await action.Should().ThrowAsync<PlayerNotFoundInCurrentGuessingPlayersException>();
    }
    
    [Fact]
    public async Task ThrowsCardNotFoundInSubmittedCardException()
    {
        var votingPlayerId = new PlayerId("votingPlayer");
        var gameRoom = GameRoomBuilder.CreateStarted(GameRoomId, "storyTellerId", new[] { votingPlayerId, new PlayerId("player3") })
            .WithStoryTellerStory("Any story")
            .WithGuessingPlayerThatSubmittedCard(votingPlayerId, "player3")
            .Build();
        var cardIdNonExistingInSubmittedCards = new CardId(Guid.Parse("8a07f339-cd2a-4ecd-8243-4baab35bc14b"));
        await _gameRoomRepository.Add(gameRoom);

        var action = async () =>
            await _commandHandler.Handle(new VoteCardCommand(votingPlayerId, GameRoomId, cardIdNonExistingInSubmittedCards));
        
        await action.Should().ThrowAsync<CardNotFoundInSubmittedCardException>();
    }
    
    [Fact]
    public async Task ThrowsPlayerCannotVoteTheirOwnSubmittedCardException()
    {
        var votingPlayerId = new PlayerId("votingPlayer");
        var otherGuessingPlayerId = new PlayerId("player3");
        var gameRoom = GameRoomBuilder.CreateStarted(GameRoomId, "storyTellerId", new[] { votingPlayerId, otherGuessingPlayerId })
            .WithStoryTellerStory("Any story")
            .WithGuessingPlayerThatSubmittedCard(votingPlayerId, "player3")
            .Build();
        var cardSubmittedByVotingPlayer = gameRoom.SubmittedCards.First(x => x.PlayerId == votingPlayerId).Card.Id;
        await _gameRoomRepository.Add(gameRoom);

        var action = async () =>
            await _commandHandler.Handle(new VoteCardCommand(votingPlayerId, GameRoomId, cardSubmittedByVotingPlayer));
        
        await action.Should().ThrowAsync<PlayerCannotVoteTheirOwnSubmittedCardException>();
    }
    
    [Fact]
    public async Task ThrowsCannotVoteIfAnyGuessingPlayerIsPendingToSubmitCardException()
    {
        var votingPlayerId = new PlayerId("votingPlayer");
        var playerPendingToSubmitCard = new PlayerId("player3");
        var gameRoom = GameRoomBuilder.CreateStarted(GameRoomId, "storyTellerId", new[] { votingPlayerId, playerPendingToSubmitCard })
            .WithStoryTellerStory("Any story")
            .WithGuessingPlayerThatSubmittedCard(votingPlayerId)
            .Build();
        var cardSubmittedByVotingPlayer = gameRoom.SubmittedCards.First(x => x.PlayerId == votingPlayerId).Card.Id;
        await _gameRoomRepository.Add(gameRoom);

        var action = async () =>
            await _commandHandler.Handle(new VoteCardCommand(votingPlayerId, GameRoomId, cardSubmittedByVotingPlayer));
        
        await action.Should().ThrowAsync<CannotVoteIfAnyPlayerIsPendingToSubmitCardException>();
    }
}