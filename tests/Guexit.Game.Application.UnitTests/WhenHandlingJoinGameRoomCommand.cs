using Guexit.Game.Application.CommandHandlers;
using Guexit.Game.Application.Commands;
using Guexit.Game.Application.Exceptions;
using Guexit.Game.Domain.Exceptions;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.Tests.Common;
using Guexit.Game.Tests.Common.Builders;

namespace Guexit.Game.Application.UnitTests;

public sealed class WhenHandlingJoinGameRoomCommand
{
    private readonly IPlayerRepository _playerRepository;
    private readonly IGameRoomRepository _gameRoomRepository;
    private readonly JoinGameRoomCommandHandler _commandHandler;

    public WhenHandlingJoinGameRoomCommand()
    {
        _playerRepository = new FakeInMemoryPlayerRepository();
        _gameRoomRepository = new FakeInMemoryGameRoomRepository();
        _commandHandler = new JoinGameRoomCommandHandler(_playerRepository, _gameRoomRepository);
    }

    [Fact]
    public async Task PlayerIsAddedToGame()
    {
        var creator = new PlayerId("player1");
        var playerJoining = new PlayerId("player2");
        var gameRoomId = new GameRoomId(Guid.NewGuid());

        await AssumePlayerInRepository(creator);
        await AssumePlayerInRepository(playerJoining);
        await _gameRoomRepository.Add(new GameRoomBuilder()
            .WithId(gameRoomId)
            .WithCreator(creator)
            .Build());

        await _commandHandler.Handle(new JoinGameRoomCommand(playerJoining.Value, gameRoomId.Value));

        var gameRoom = await _gameRoomRepository.GetBy(gameRoomId);
        gameRoom.Should().NotBeNull();
        gameRoom!.PlayerIds.Should().BeEquivalentTo(new[] { creator, playerJoining });
    }

    [Fact]
    public async Task ThrowsCannotJoinStartedGameIfGameRoomStatusIsDifferentFromNotStarted()
    {
        var creator = new PlayerId("player1");
        var playerThatJoined = new PlayerId("player2");
        var playerJoining = new PlayerId("player3");
        var gameRoomId = new GameRoomId(Guid.NewGuid());
        await AssumePlayerInRepository(creator);
        await AssumePlayerInRepository(playerJoining);
        var gameRoom = new GameRoomBuilder()
            .WithId(gameRoomId)
            .WithCreator(creator)
            .WithPlayersThatJoined(playerThatJoined, "otherPlayerId")
            .WithValidDeckAssigned()
            .Started()
            .Build();
        await _gameRoomRepository.Add(gameRoom);

        var action = async () => await _commandHandler.Handle(new JoinGameRoomCommand(playerJoining.Value, gameRoomId.Value));

        await action.Should().ThrowAsync<JoinStartedGameException>()
            .WithMessage($"Player with id {playerJoining.Value} cannot join game with id {gameRoomId.Value} because game already started");
    }

    [Fact]
    public async Task DoesNotRaisePlayerJoinedEventIfPlayerWasAlreadyInGame()
    {
        var creator = new PlayerId("player1");
        var playerJoining = new PlayerId("player2");
        var gameRoomId = new GameRoomId(Guid.NewGuid());

        await AssumePlayerInRepository(creator);
        await AssumePlayerInRepository(playerJoining);
        await AssumeGameInRepositoryWithPlayersThatJoined(gameRoomId, creator, playersThatJoined: playerJoining);

        await _commandHandler.Handle(new JoinGameRoomCommand(playerJoining.Value, gameRoomId.Value));

        var gameRoom = await _gameRoomRepository.GetBy(gameRoomId);
        gameRoom.Should().NotBeNull();
        gameRoom!.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public async Task ThrowsPlayerNotFoundExceptionIfPlayerDoesntExist()
    {
        var nonExistingPlayer = new PlayerId("player1");

        var action = async () => await _commandHandler.Handle(new JoinGameRoomCommand(nonExistingPlayer.Value, Guid.NewGuid()));

        await action.Should().ThrowAsync<PlayerNotFoundException>()
            .WithMessage("Player with id player1 not found.");
    }

    [Fact]
    public async Task ThrowsGameRoomNotFoundExceptionIfGameRoomDoesntExist()
    {
        var creator = new PlayerId("player1");
        var nonExistingGameRoomId = Guid.NewGuid();
        await AssumePlayerInRepository(creator);

        var action = async () => await _commandHandler.Handle(new JoinGameRoomCommand(creator.Value, nonExistingGameRoomId));

        await action.Should().ThrowAsync<GameRoomNotFoundException>()
            .WithMessage($"Game room with id {nonExistingGameRoomId} not found.");
    }

    [Fact]
    public async Task ThrowsCannotJoinFullGameRoomException()
    {
        var creator = new PlayerId("creator");
        var playerJoining = new PlayerId("playerJoining");
        var gameRoomId = new GameRoomId(Guid.NewGuid());
        var alreadyFullGameRoom = new GameRoomBuilder()
            .WithId(gameRoomId)
            .WithCreator(creator)
            .WithPlayersThatJoined(Enumerable.Range(0, 9).Select(x => new PlayerId($"invitedPlayer{x}")).ToArray())
            .Build();

        await AssumePlayerInRepository(creator);
        await AssumePlayerInRepository(playerJoining);
        await _gameRoomRepository.Add(alreadyFullGameRoom);

        var act = async () => await _commandHandler.Handle(new JoinGameRoomCommand(playerJoining.Value, gameRoomId.Value));

        await act.Should().ThrowAsync<CannotJoinFullGameRoomException>()
            .WithMessage($"Player with id {playerJoining.Value} cannot join game room with id {gameRoomId.Value} because it is already full");
    }

    [Fact]
    public async Task DoesNotThrowExceptionIfGameIsFullButPlayerJoiningWasAlreadyIn()
    {
        var creator = new PlayerId("creator");
        var gameRoomId = new GameRoomId(Guid.NewGuid());
        var alreadyFullGameRoom = new GameRoomBuilder()
            .WithId(gameRoomId)
            .WithCreator(creator)
            .WithPlayersThatJoined(Enumerable.Range(0, 9).Select(x => new PlayerId($"invitedPlayer{x}")).ToArray())
            .Build();

        await AssumePlayerInRepository(creator);
        await _gameRoomRepository.Add(alreadyFullGameRoom);

        await _commandHandler.Handle(new JoinGameRoomCommand(creator.Value, gameRoomId.Value));

        var gameRoom = await _gameRoomRepository.GetBy(gameRoomId);
        gameRoom.Should().NotBeNull();
        gameRoom!.PlayerIds.Should().Contain(creator);
    }

    private async Task AssumeGameInRepositoryWithPlayersThatJoined(GameRoomId gameRoomId, PlayerId creatorId, params PlayerId[] playersThatJoined)
    {
        var gameRoom = new GameRoomBuilder()
            .WithId(gameRoomId)
            .WithCreator(creatorId)
            .WithPlayersThatJoined(playersThatJoined)
            .Build();

        await _gameRoomRepository.Add(gameRoom);
    }

    private async Task AssumePlayerInRepository(PlayerId creatorId)
    {
        await _playerRepository.Add(
            new Player(creatorId, "anyusername@acme.com")
        );
    }
}
