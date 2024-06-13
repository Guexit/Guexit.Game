using Guexit.Game.Application.CommandHandlers;
using Guexit.Game.Application.Commands;
using Guexit.Game.Application.Exceptions;
using Guexit.Game.Domain.Exceptions;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.GameRoomAggregate.Events;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.Tests.Common.Builders;
using Guexit.Game.Tests.Common.ObjectMothers;

namespace Guexit.Game.Application.UnitTests;

public sealed class WhenHandlingVoteCardCommand
{
    private static readonly GameRoomId GameRoomId = new(Guid.NewGuid());

    private readonly IGameRoomRepository _gameRoomRepository;
    private readonly VoteCardCommandHandler _commandHandler;
    
    public WhenHandlingVoteCardCommand()
    {
        _gameRoomRepository = new FakeInMemoryGameRoomRepository();
        _commandHandler = new VoteCardCommandHandler(_gameRoomRepository);
    }

    [Fact]
    public async Task VotingPlayerIsAddedToSubmittedCardVoters()
    {
        var votingPlayerId = new PlayerId("votingPlayer");
        var gameRoom = GameRoomBuilder.CreateStarted(GameRoomId, "storyTellerId", [votingPlayerId, "player3"])
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
        var gameRoom = GameRoomBuilder.CreateStarted(GameRoomId, storyTellerId, [votingPlayerId1, votingPlayerId2])
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
        var gameRoom = GameRoomBuilder.CreateStarted(GameRoomId, storyTellerId, [votingPlayerId1, votingPlayerId2])
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
    public async Task GuessingPlayerVotedEventIsRaised()
    {
        var votingPlayerId = new PlayerId("votingPlayer");
        var gameRoom = GameRoomBuilder.CreateStarted(GameRoomId, "storyTellerId", [votingPlayerId, new PlayerId("player3")])
            .WithStoryTellerStory("Any story")
            .WithGuessingPlayerThatSubmittedCard(votingPlayerId, "player3")
            .Build();
        var votedCardId = gameRoom.SubmittedCards.First(x => x.PlayerId != votingPlayerId).Card.Id;
        await _gameRoomRepository.Add(gameRoom);

        await _commandHandler.Handle(new VoteCardCommand(votingPlayerId, GameRoomId, votedCardId));

        gameRoom.DomainEvents.OfType<GuessingPlayerVoted>().Should().HaveCount(1);
        var @event = gameRoom.DomainEvents.OfType<GuessingPlayerVoted>().Single();
        @event.GameRoomId.Should().Be(gameRoom.Id);
        @event.PlayerId.Should().Be(votingPlayerId);
        @event.SelectedCardId.Should().Be(votedCardId);
    }
    
    [Fact]
    public async Task GuessingPlayersReceiveOnePointPerEachGuesserVoteAndStoryTellerDoNotReceiveIfNoOneVotedTheirs()
    {
        var storyTellerId = new PlayerId("storyTellerId");
        var votingPlayerId1 = new PlayerId("votingPlayer1");
        var votingPlayerId2 = new PlayerId("votingPlayer2");
        var gameRoom = GameRoomBuilder.CreateStarted(GameRoomId, storyTellerId, [votingPlayerId1, votingPlayerId2])
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
    public async Task VotingScoresComputedEventIsRaisedIfAllGuessingPlayersHaveVoted()
    {
        var storyTellerId = new PlayerId("storyTellerId");
        var votingPlayerId1 = new PlayerId("votingPlayer1");
        var votingPlayerId2 = new PlayerId("votingPlayer2");
        var gameRoom = GameRoomBuilder.CreateStarted(GameRoomId, storyTellerId, [votingPlayerId1, votingPlayerId2])
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
    public async Task ShiftsToNextRoundIfAllGuessingPlayersHaveVoted()
    {
        var pastStoryTeller = new PlayerId("storyTellerId");
        var nextStoryTeller = new PlayerId("votingPlayer1");

        var votingPlayerId2 = new PlayerId("votingPlayer2");

        var gameRoom = GameRoomBuilder.CreateStarted(GameRoomId, pastStoryTeller, [nextStoryTeller, votingPlayerId2])
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
        gameRoom.CurrentCardReRolls.Should().BeEmpty();
        gameRoom.PlayerHands.Should().AllSatisfy(x => x.Cards.Should().HaveCount(GameRoom.PlayerHandSize));
        
        gameRoom.DomainEvents.OfType<NewRoundStarted>().Should().HaveCount(1);
        var newRoundStartedEvent = gameRoom.DomainEvents.OfType<NewRoundStarted>().Single();
        newRoundStartedEvent.GameRoomId.Should().Be(GameRoomId);
    }

    [Fact]
    public async Task RaisesReserveCardsForReRollDiscardedIfAnyCardReRollWasNotCompletedAndShiftsToNextRound()
    {
        var player1 = new PlayerId("thanos");
        var player2 = new PlayerId("spiderman");
        var playerPendingToVote = new PlayerId("ironman");
        var gameRoom = GameRoomBuilder.CreateStarted(GameRoomId, player1, [player2, playerPendingToVote])
            .WithStoryTellerStory("Any Story")
            .WithGuessingPlayerThatSubmittedCard(player2, playerPendingToVote)
            .WithVote(votingPlayer: player2, cardSubmittedBy: player1)
            .WithPlayerThatReservedCardsForReRoll(player2)
            .WithPlayerThatReservedCardsForReRoll(playerPendingToVote)
            .Build();
        await _gameRoomRepository.Add(gameRoom);
        var notCompletedCardReRolls = gameRoom.CurrentCardReRolls.ToArray(); 
        
        var anyCard = gameRoom.SubmittedCards.First(x => x.PlayerId != playerPendingToVote).Card.Id;
        
        await _commandHandler.Handle(new VoteCardCommand(playerPendingToVote, GameRoomId, anyCard));

        gameRoom.DomainEvents.OfType<ReserveCardsForReRollDiscarded>().Should().HaveCount(1);
        var @event = gameRoom.DomainEvents.OfType<ReserveCardsForReRollDiscarded>().First();
        @event.GameRoomId.Should().Be(GameRoomId);
        @event.UnusedCardImageUrls.Should()
            .BeEquivalentTo(notCompletedCardReRolls.SelectMany(x => x.ReservedCards).Select(card => card.Url));
    }
    
    [Fact]
    public async Task RaisesReserveCardsForReRollDiscardedIfAnyCardReRollWasNotCompletedAndGameIsFinished()
    {
        var player1 = new PlayerId("thanos");
        var player2 = new PlayerId("spiderman");
        var player3 = new PlayerId("ironman");
        var gameRoom = GameRoomObjectMother.OneVotePendingToFinish(GameRoomId, player1, [player2, player3]);
        
        gameRoom.ReserveCardsForReRoll(player2, Enumerable.Range(0, 3).Select(_ => new CardBuilder().Build()).ToArray());
        
        var notCompletedCardReRolls = gameRoom.CurrentCardReRolls.ToArray();
        
        await _gameRoomRepository.Add(gameRoom);
        
        var playerPendingToVote = gameRoom.GetCurrentGuessingPlayerIds().Except(gameRoom.SubmittedCards.SelectMany(x => x.Voters)).Single();
        var anyCard = gameRoom.SubmittedCards.First(x => x.PlayerId != playerPendingToVote).Card.Id;
        await _commandHandler.Handle(new VoteCardCommand(playerPendingToVote, GameRoomId, anyCard));

        gameRoom.DomainEvents.OfType<ReserveCardsForReRollDiscarded>().Should().HaveCount(1);
        var @event = gameRoom.DomainEvents.OfType<ReserveCardsForReRollDiscarded>().First();
        @event.GameRoomId.Should().Be(GameRoomId);
        @event.UnusedCardImageUrls.Should()
            .BeEquivalentTo(notCompletedCardReRolls.SelectMany(x => x.ReservedCards).Select(card => card.Url));
    }
    
    [Fact]
    public async Task DoesNotRaiseReserveCardsForReRollDiscardedCardReRollNotCompleted()
    {
        var player1 = new PlayerId("thanos");
        var player2 = new PlayerId("spiderman");
        var playerPendingToVote = new PlayerId("ironman");
        var gameRoom = GameRoomBuilder.CreateStarted(GameRoomId, player1, [player2, playerPendingToVote])
            .WithStoryTellerStory("Any Story")
            .WithGuessingPlayerThatSubmittedCard(player2, playerPendingToVote)
            .WithVote(votingPlayer: player2, cardSubmittedBy: player1)
            .Build();
        await _gameRoomRepository.Add(gameRoom);
        
        var anyCard = gameRoom.SubmittedCards.First(x => x.PlayerId != playerPendingToVote).Card.Id;
        
        await _commandHandler.Handle(new VoteCardCommand(playerPendingToVote, GameRoomId, anyCard));

        gameRoom.DomainEvents.OfType<ReserveCardsForReRollDiscarded>().Should().BeEmpty();
    }

    [Fact]
    public async Task GameIsFinishedWhenEveryPlayerHasBeenStoryTellerOnce()
    {
        var player1 = new PlayerId("thanos");
        var player2 = new PlayerId("spiderman");
        var player3 = new PlayerId("ironman");
        var gameRoom = GameRoomObjectMother.OneVotePendingToFinish(GameRoomId, player1, [player2, player3]);
        await _gameRoomRepository.Add(gameRoom);

        var playerPendingToVote = gameRoom.GetCurrentGuessingPlayerIds().Except(gameRoom.SubmittedCards.SelectMany(x => x.Voters)).Single();
        var card = gameRoom.SubmittedCards.First(x => x.PlayerId != playerPendingToVote).Card.Id;

        await _commandHandler.Handle(new VoteCardCommand(playerPendingToVote, GameRoomId, card));

        gameRoom.Status.Should().Be(GameStatus.Finished);
        gameRoom.DomainEvents.OfType<GameFinished>().Should().HaveCount(1);
        gameRoom.DomainEvents.OfType<GameFinished>().Single().GameRoomId.Should().Be(GameRoomId);
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

        await action.Should().ThrowAsync<InvalidOperationForNotInProgressGameException>();
    }
    
    [Fact]
    public async Task ThrowsPlayerNotFoundInCurrentGuessingPlayersExceptionException()
    {
        var storyTellerId = new PlayerId("storyTellerId");
        var anyCardId = new CardId(Guid.NewGuid());
        await _gameRoomRepository.Add(GameRoomBuilder.CreateStarted(GameRoomId, storyTellerId, ["player2", "player3"])
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
        var gameRoom = GameRoomBuilder.CreateStarted(GameRoomId, "storyTellerId", [votingPlayerId, new PlayerId("player3")
            ])
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
        var gameRoom = GameRoomBuilder.CreateStarted(GameRoomId, "storyTellerId", [votingPlayerId, otherGuessingPlayerId
            ])
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
        var gameRoom = GameRoomBuilder.CreateStarted(GameRoomId, "storyTellerId", [votingPlayerId, playerPendingToSubmitCard
            ])
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