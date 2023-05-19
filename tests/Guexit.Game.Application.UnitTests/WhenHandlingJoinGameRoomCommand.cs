using Guexit.Game.Application.CommandHandlers;
using Guexit.Game.Application.Commands;
using Guexit.Game.Application.Exceptions;
using Guexit.Game.Domain.Exceptions;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.Tests.Common;

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
        _commandHandler = new JoinGameRoomCommandHandler(
            Substitute.For<IUnitOfWork>(),
            _playerRepository,
            _gameRoomRepository
        );
    }

    [Fact]
    public async Task PlayerIsAddedToGame()
    {
        var creator = new PlayerId("player1");
        var playerJoining = new PlayerId("player2");
        var gameRoomId = new GameRoomId(Guid.NewGuid());

        await AssumePlayerInRepository(creator);
        await AssumePlayerInRepository(playerJoining);
        await AssumeGameInRepository(gameRoomId, creator);

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
            .Started()
            .Build();
        await _gameRoomRepository.Add(gameRoom);

        var action = async () => await _commandHandler.Handle(new JoinGameRoomCommand(playerJoining.Value, gameRoomId.Value));

        await action.Should().ThrowAsync<JoinStartedGameException>();
    }

    [Fact]
    public async Task ThrowsPlayerIsAlreadyInGameRoomExceptionIfPlayerAlreadyJoined()
    {
        var creator = new PlayerId("player1");
        var playerJoining = new PlayerId("player2");
        var gameRoomId = new GameRoomId(Guid.NewGuid());

        await AssumePlayerInRepository(creator);
        await AssumePlayerInRepository(playerJoining);
        await AssumeGameInRepositoryWithPlayersThatJoined(gameRoomId, creator, playersThatJoined: playerJoining);

        var action = async () => await _commandHandler.Handle(new JoinGameRoomCommand(playerJoining.Value, gameRoomId.Value));

        await action.Should().ThrowAsync<PlayerIsAlreadyInGameRoomException>()
            .WithMessage("Player with id player2 is already in game room.");
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

    private async Task AssumeGameInRepository(GameRoomId gameRoomId, PlayerId creatorId)
    {
        var game = new GameRoom(gameRoomId, creatorId, new DateTimeOffset(2023, 1, 1, 1, 2, 3, TimeSpan.Zero));
        await _gameRoomRepository.Add(game);
    }

    private async Task AssumeGameInRepositoryWithPlayersThatJoined(GameRoomId gameRoomId, PlayerId creatorId, params PlayerId[] playersThatJoined)
    {
        var game = new GameRoom(gameRoomId, creatorId, new DateTimeOffset(2023, 1, 1, 1, 2, 3, TimeSpan.Zero));
        foreach(var playerId in playersThatJoined)
        {
            game.Join(playerId);
        }

        await _gameRoomRepository.Add(game);
    }

    private async Task AssumePlayerInRepository(PlayerId creatorId)
    {
        await _playerRepository.Add(
            new Player(creatorId, string.Empty)
        );
    }
}
