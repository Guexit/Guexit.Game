using TryGuessIt.Game.Application.Commands;
using TryGuessIt.Game.Domain.Model.GameRoom;
using TryGuessIt.Game.Domain.Model.PlayerAggregate;

namespace TryGuessIt.Game.Application.CommandHandlers;

public sealed class CreateGameRoomCommandHandler : CommandHandler<CreateGameRoomCommand>
{
    private readonly IPlayerRepository _playerRepository;
    private readonly IGameRoomRepository _gameRoomRepository;

    public CreateGameRoomCommandHandler(
        IUnitOfWork unitOfWork, 
        IPlayerRepository _playerRepository, 
        IGameRoomRepository _gameRoomRepository) : base(unitOfWork)
    {
        this._playerRepository = _playerRepository;
        this._gameRoomRepository = _gameRoomRepository;
    }

    protected override ValueTask Process(CreateGameRoomCommand command, CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }
}
