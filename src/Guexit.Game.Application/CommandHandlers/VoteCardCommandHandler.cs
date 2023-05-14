using Guexit.Game.Application.Commands;
using Guexit.Game.Application.Exceptions;
using Guexit.Game.Domain.Model.GameRoomAggregate;

namespace Guexit.Game.Application.CommandHandlers;

public sealed class VoteCardCommandHandler : CommandHandler<VoteCardCommand>
{
    private readonly IGameRoomRepository _gameRoomRepository;

    public VoteCardCommandHandler(IUnitOfWork unitOfWork, IGameRoomRepository gameRoomRepository) : base(unitOfWork)
    {
        _gameRoomRepository = gameRoomRepository;
    }

    protected override async ValueTask Process(VoteCardCommand command, CancellationToken ct)
    {
        var gameRoom = await _gameRoomRepository.GetBy(command.GameRoomId, ct)
            ?? throw new GameRoomNotFoundException(command.GameRoomId);
        
        gameRoom.VoteCard(command.VotingPlayerId, command.SubmittedCardId);
    }
}