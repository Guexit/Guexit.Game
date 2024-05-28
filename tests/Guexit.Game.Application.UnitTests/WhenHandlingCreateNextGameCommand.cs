using Guexit.Game.Application.CommandHandlers;
using Guexit.Game.Application.Commands;
using Guexit.Game.Application.Exceptions;
using Guexit.Game.Domain;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.GameRoomAggregate.Events;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.Tests.Common.Builders;
using Guexit.Game.Tests.Common.ObjectMothers;

namespace Guexit.Game.Application.UnitTests;

public sealed class WhenHandlingCreateNextGameCommand
{
    private static readonly DateTimeOffset Now = new(2023, 12, 24, 17, 23, 55, TimeSpan.Zero);
    
    private readonly IPlayerRepository _playerRepository;
    private readonly IGameRoomRepository _gameRoomRepository;
    private readonly IGuidProvider _guidProvider;
    private readonly ISystemClock _clock;
    private readonly CreateNextGameRoomCommandHandler _commandHandler;

    public WhenHandlingCreateNextGameCommand()
    {
        _playerRepository = new FakeInMemoryPlayerRepository();
        _gameRoomRepository = new FakeInMemoryGameRoomRepository();
        _guidProvider = Substitute.For<IGuidProvider>();
        _clock = Substitute.For<ISystemClock>();
        _commandHandler = new CreateNextGameRoomCommandHandler(_playerRepository, _gameRoomRepository, _guidProvider, _clock);
        
        _clock.UtcNow.Returns(Now);
    }

    [Fact]
    public async Task CreatesAnotherGameRoomAndAssignsItsIdToCurrentGameRoom()
    {
        var finishedGameRoomId = new GameRoomId(Guid.NewGuid());
        var nextGameRoomId = new GameRoomId(Guid.NewGuid());
        var playerId = new PlayerId("player1");
        
        await _playerRepository.Add(new PlayerBuilder().WithId(playerId).Build());
        await _gameRoomRepository.Add(GameRoomObjectMother.Finished(finishedGameRoomId, playerId, ["player2", "player3"]));

        _guidProvider.NewGuid().Returns(nextGameRoomId.Value);
        
        await _commandHandler.Handle(new CreateNextGameRoomCommand(playerId, finishedGameRoomId));

        var finishedGameRoom = await _gameRoomRepository.GetBy(finishedGameRoomId);
        finishedGameRoom.Should().NotBeNull();
        finishedGameRoom!.NextGameRoomId.Should().Be(nextGameRoomId);

        var nextGameRoom = await _gameRoomRepository.GetBy(nextGameRoomId);
        nextGameRoom.Should().NotBeNull();
        nextGameRoom!.Id.Should().Be(finishedGameRoom.NextGameRoomId);
        nextGameRoom.CreatedAt.Should().Be(_clock.UtcNow);
        nextGameRoom.CreatedBy.Should().Be(playerId);
        nextGameRoom.PlayerIds.Should().HaveCount(1).And.Subject.Single().Should().Be(playerId);
        nextGameRoom.Status.Should().Be(GameStatus.NotStarted);
    }

    [Fact]
    public async Task DoesNotCreateANewOneIfWasAlreadyLinked()
    {
        var finishedGameRoomId = new GameRoomId(Guid.Parse("12403be9-75dd-458a-9e70-16badde8d931"));
        var alreadyLinkedGameRoomId = new GameRoomId(Guid.Parse("32403be9-75dd-458a-9e70-16badde8d933"));
        var playerId = new PlayerId("player1");
        
        await _playerRepository.Add(new PlayerBuilder().WithId(playerId).Build());
        await _gameRoomRepository.Add(new GameRoomBuilder().WithId(alreadyLinkedGameRoomId).WithCreator(playerId).WithPlayersThatJoined(["player2", "player3"]).Build());
        await _gameRoomRepository.Add(GameRoomObjectMother.Finished(finishedGameRoomId, playerId, ["player2", "player3"], alreadyLinkedGameRoomId));

        var nextGameRoomId = new GameRoomId(Guid.Parse("22403be9-75dd-458a-9e70-16badde8d932"));
        _guidProvider.NewGuid().Returns(nextGameRoomId.Value);
        
        await _commandHandler.Handle(new CreateNextGameRoomCommand(playerId, finishedGameRoomId));
        
        var gameRoom = await _gameRoomRepository.GetBy(finishedGameRoomId);
        gameRoom.Should().NotBeNull();
        gameRoom!.NextGameRoomId.Should().Be(alreadyLinkedGameRoomId);

        var createdNextGameRoo = await _gameRoomRepository.GetBy(nextGameRoomId);
        createdNextGameRoo.Should().BeNull();
        gameRoom.DomainEvents.OfType<NextGameRoomLinked>().Should().BeEmpty();
    }

    [Fact]
    public async Task NextGameRoomLinkedEventIsRaised()
    {
        var finishedGameRoomId = new GameRoomId(Guid.NewGuid());
        var nextGameRoomId = new GameRoomId(Guid.NewGuid());
        var playerId = new PlayerId("player1");
        
        await _playerRepository.Add(new PlayerBuilder().WithId(playerId).Build());
        await _gameRoomRepository.Add(GameRoomObjectMother.Finished(finishedGameRoomId, playerId, ["player2", "player3"]));

        _guidProvider.NewGuid().Returns(nextGameRoomId.Value);
        
        await _commandHandler.Handle(new CreateNextGameRoomCommand(playerId, finishedGameRoomId));

        var finishedGameRoom = await _gameRoomRepository.GetBy(finishedGameRoomId);
        finishedGameRoom.Should().NotBeNull();
        finishedGameRoom!.DomainEvents.OfType<NextGameRoomLinked>().Should().HaveCount(1);

        var @event = finishedGameRoom.DomainEvents.OfType<NextGameRoomLinked>().Single();
        @event.FinishedGameRoomId.Should().Be(finishedGameRoomId);
        @event.NextGameRoomId.Should().Be(nextGameRoomId);
    }

    [Fact]
    public async Task JoinsPlayerToNextGameIfItWasAlreadyCreated()
    {
        var finishedGameRoomId = new GameRoomId(Guid.Parse("12403be9-75dd-458a-9e70-16badde8d931"));
        var alreadyLinkedGameRoomId = new GameRoomId(Guid.Parse("32403be9-75dd-458a-9e70-16badde8d933"));
        var player1 = new PlayerBuilder().WithId("player1").Build();
        var player2 = new PlayerBuilder().WithId("player2").Build();
        var player3 = new PlayerBuilder().WithId("player3").Build();
        var finishedGameRoom = GameRoomObjectMother.Finished(finishedGameRoomId, player1.Id, invitedPlayers: [player2.Id, player3.Id], alreadyLinkedGameRoomId);
        var nextGameRoom = new GameRoomBuilder()
            .WithId(alreadyLinkedGameRoomId)
            .WithCreator(player1.Id)
            .WithPlayersThatJoined(player2.Id)
            .Build();

        await _playerRepository.Add(player1);
        await _playerRepository.Add(player2);
        await _playerRepository.Add(player3);
        await _gameRoomRepository.Add(finishedGameRoom);
        
        await _gameRoomRepository.Add(nextGameRoom);

        await _commandHandler.Handle(new CreateNextGameRoomCommand(player3.Id, finishedGameRoomId));

        nextGameRoom.PlayerIds.Should().Contain(player3.Id);
    }

    [Fact]
    public async Task ThrowsPlayerNotFoundExceptionIfItDoesNotExist()
    {
        var command = new CreateNextGameRoomCommand("nonExistingPlayerId", Guid.NewGuid());

        var action = async () => await _commandHandler.Handle(command);

        await action.Should().ThrowAsync<PlayerNotFoundException>();
    }
    
    [Fact]
    public async Task ThrowsGameRoomNotFoundExceptionIfItDoesNotExist()
    {
        var playerId = new PlayerId("1");
        var nonExistingGameRoomId = new GameRoomId(Guid.NewGuid());
        
        await _playerRepository.Add(new PlayerBuilder().WithId(playerId).Build());
        
        var action = async () => await _commandHandler.Handle(new CreateNextGameRoomCommand(playerId, nonExistingGameRoomId));

        await action.Should().ThrowAsync<GameRoomNotFoundException>();
    }
}