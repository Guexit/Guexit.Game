using Guexit.Game.Application.CommandHandlers;
using Guexit.Game.Application.Commands;
using Guexit.Game.Application.Exceptions;
using Guexit.Game.Domain.Exceptions;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.GameRoomAggregate.Events;
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
        gameRoom.DomainEvents.OfType<VotingScoresComputed>().Should().HaveCount(0);
    }

    [Fact]
    public async Task RoundScoreIsCalculated()
    {
        var storyTellerId = new PlayerId("storyTellerId");
        var votingPlayerId1 = new PlayerId("votingPlayer1");
        var votingPlayerId2 = new PlayerId("votingPlayer2");
        var gameRoom = GameRoomBuilder.CreateStarted(GameRoomId, storyTellerId, new[] { votingPlayerId1, votingPlayerId2 })
            .WithStoryTellerStory("Any story")
            .WithGuessingPlayerThatSubmittedCard(votingPlayerId1, votingPlayerId2)
            .Build();
        var cardSubmittedByPlayer2 = gameRoom.SubmittedCards.First(x => x.PlayerId == votingPlayerId2).Card.Id;
        var storyTellerCard = gameRoom.SubmittedCards.First(x => x.PlayerId == storyTellerId).Card.Id;
        await _gameRoomRepository.Add(gameRoom);

        await _commandHandler.Handle(new VoteCardCommand(votingPlayerId1, GameRoomId, cardSubmittedByPlayer2));
        await _commandHandler.Handle(new VoteCardCommand(votingPlayerId2, GameRoomId, storyTellerCard));

        gameRoom.FinishedRounds.Should().HaveCount(1);
        var finishedRound = gameRoom.FinishedRounds.Single();
        finishedRound.GetScoredPointsOf(storyTellerId).Should().Be(new Points(3));
        finishedRound.GetScoredPointsOf(votingPlayerId1).Should().Be(new Points(0));
        finishedRound.GetScoredPointsOf(votingPlayerId2).Should().Be(new Points(4));
    }

    [Fact]
    public async Task StoryTellerDoNotReceiveAnyPointIfAllGuessersVotedTheirCard()
    {
        var storyTellerId = new PlayerId("storyTellerId");
        var votingPlayerId1 = new PlayerId("votingPlayer1");
        var votingPlayerId2 = new PlayerId("votingPlayer2");
        var gameRoom = GameRoomBuilder.CreateStarted(GameRoomId, storyTellerId, new[] { votingPlayerId1, votingPlayerId2 })
            .WithStoryTellerStory("Any story")
            .WithGuessingPlayerThatSubmittedCard(votingPlayerId1, votingPlayerId2)
            .Build();
        var storyTellerCard = gameRoom.SubmittedCards.First(x => x.PlayerId == storyTellerId).Card.Id;
        await _gameRoomRepository.Add(gameRoom);

        await _commandHandler.Handle(new VoteCardCommand(votingPlayerId1, GameRoomId, storyTellerCard));
        await _commandHandler.Handle(new VoteCardCommand(votingPlayerId2, GameRoomId, storyTellerCard));

        gameRoom.FinishedRounds.Should().HaveCount(1);
        var finishedRound = gameRoom.FinishedRounds.Single();
        finishedRound.GetScoredPointsOf(storyTellerId).Should().Be(Points.Zero);
        finishedRound.GetScoredPointsOf(votingPlayerId1).Should().Be(new Points(3));
        finishedRound.GetScoredPointsOf(votingPlayerId2).Should().Be(new Points(3));
    }

    [Fact]
    public async Task GuessingPlayersReceiveOnePointPerEachGuesserVoteAndStoryTellerDoNotReceiveIfNoOneVotedTheirs()
    {
        var storyTellerId = new PlayerId("storyTellerId");
        var votingPlayerId1 = new PlayerId("votingPlayer1");
        var votingPlayerId2 = new PlayerId("votingPlayer2");
        var gameRoom = GameRoomBuilder.CreateStarted(GameRoomId, storyTellerId, new[] { votingPlayerId1, votingPlayerId2 })
            .WithStoryTellerStory("Any story")
            .WithGuessingPlayerThatSubmittedCard(votingPlayerId1, votingPlayerId2)
            .Build();
        var storyTellerCard = gameRoom.SubmittedCards.First(x => x.PlayerId == storyTellerId).Card.Id;
        var guesser1Card = gameRoom.SubmittedCards.First(x => x.PlayerId == votingPlayerId1).Card.Id;
        var guesser2Card = gameRoom.SubmittedCards.First(x => x.PlayerId == votingPlayerId2).Card.Id;
        await _gameRoomRepository.Add(gameRoom);

        await _commandHandler.Handle(new VoteCardCommand(votingPlayerId1, GameRoomId, guesser2Card));
        await _commandHandler.Handle(new VoteCardCommand(votingPlayerId2, GameRoomId, guesser1Card));

        gameRoom.FinishedRounds.Should().HaveCount(1);
        var finishedRound = gameRoom.FinishedRounds.Single();
        finishedRound.GetScoredPointsOf(storyTellerId).Should().Be(Points.Zero);
        finishedRound.GetScoredPointsOf(votingPlayerId1).Should().Be(new Points(1));
        finishedRound.GetScoredPointsOf(votingPlayerId2).Should().Be(new Points(1));
    }

    [Fact]
    public async Task VotingScoresComputedIsRaisedWhenAllGuessingPlayersHaveVoted()
    {
        var storyTellerId = new PlayerId("storyTellerId");
        var votingPlayerId1 = new PlayerId("votingPlayer1");
        var votingPlayerId2 = new PlayerId("votingPlayer2");
        var gameRoom = GameRoomBuilder.CreateStarted(GameRoomId, storyTellerId, new[] { votingPlayerId1, votingPlayerId2 })
            .WithStoryTellerStory("Any story")
            .WithGuessingPlayerThatSubmittedCard(votingPlayerId1, votingPlayerId2)
            .Build();
        var cardSubmittedByPlayer2 = gameRoom.SubmittedCards.First(x => x.PlayerId == storyTellerId).Card.Id;
        var storyTellerCard = gameRoom.SubmittedCards.First(x => x.PlayerId != storyTellerId).Card.Id;
        await _gameRoomRepository.Add(gameRoom);

        await _commandHandler.Handle(new VoteCardCommand(votingPlayerId1, GameRoomId, cardSubmittedByPlayer2));
        await _commandHandler.Handle(new VoteCardCommand(votingPlayerId2, GameRoomId, storyTellerCard));
        
        gameRoom.DomainEvents.OfType<VotingScoresComputed>().Should().HaveCount(1).And.Subject
            .Single().GameRoomId.Should().Be(GameRoomId);
    }


    [Fact]
    public async Task ShiftToNextTurnOnLastVoteReceived()
    {
        var pastStoryTeller = new PlayerId("storyTellerId");
        var nextStoryTeller = new PlayerId("votingPlayer1");

        var votingPlayerId2 = new PlayerId("votingPlayer2");

        var gameRoom = GameRoomBuilder.CreateStarted(GameRoomId, pastStoryTeller, new[] { nextStoryTeller, votingPlayerId2 })
            .WithStoryTellerStory("Any story")
            .WithGuessingPlayerThatSubmittedCard(nextStoryTeller, votingPlayerId2)
            .Build();
        var cardSubmittedByPlayer2 = gameRoom.SubmittedCards.First(x => x.PlayerId == pastStoryTeller).Card.Id;
        var storyTellerCard = gameRoom.SubmittedCards.First(x => x.PlayerId != pastStoryTeller).Card.Id;
        await _gameRoomRepository.Add(gameRoom);

        await _commandHandler.Handle(new VoteCardCommand(nextStoryTeller, GameRoomId, cardSubmittedByPlayer2));
        await _commandHandler.Handle(new VoteCardCommand(votingPlayerId2, GameRoomId, storyTellerCard));

        gameRoom.CurrentStoryTeller.PlayerId.Should().Be(nextStoryTeller);
        gameRoom.CurrentStoryTeller.Story.Should().Be(StoryTeller.Empty.Story);
        gameRoom.SubmittedCards.Should().BeEmpty();
        gameRoom.PlayerHands.Should().AllSatisfy(x => x.Cards.Should().HaveCount(GameRoom.CardsInHandPerPlayer));
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

        await action.Should().ThrowAsync<VoteCardToNotInProgressGameRoomException>();
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
        
        await action.Should().ThrowAsync<CardNotFoundInSubmittedCardsException>();
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