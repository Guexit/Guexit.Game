using Guexit.Game.Application.Commands;
using Guexit.Game.Application.Exceptions;
using Guexit.Game.Domain.Model.GameRoomAggregate;

namespace Guexit.Game.Application.CommandHandlers;

public sealed class SubmitGuessingPlayerCardCommandHandler : CommandHandler<SubmitGuessingPlayerCardCommand>
{
    private readonly IGameRoomRepository _gameRoomRepository;

    public SubmitGuessingPlayerCardCommandHandler(IUnitOfWork unitOfWork, IGameRoomRepository gameRoomRepository) : base(unitOfWork)
    {
        _gameRoomRepository = gameRoomRepository;
    }

    protected override async ValueTask Process(SubmitGuessingPlayerCardCommand command, CancellationToken ct)
    {
        var gameRoom = await _gameRoomRepository.GetBy(command.GameRoomId, ct)
            ?? throw new GameRoomNotFoundException(command.GameRoomId);

        gameRoom.SubmitGuessingPlayerCard(command.PlayerId, command.CardId);
    }
}
