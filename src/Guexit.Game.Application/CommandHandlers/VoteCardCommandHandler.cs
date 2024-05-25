using Guexit.Game.Application.Commands;
using Guexit.Game.Application.Exceptions;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.ImageAggregate;
using Mediator;

namespace Guexit.Game.Application.CommandHandlers;

public sealed class VoteCardCommandHandler : ICommandHandler<VoteCardCommand>
{
    private readonly IGameRoomRepository _gameRoomRepository;
    private readonly IImageRepository _imageRepository;

    public VoteCardCommandHandler(IGameRoomRepository gameRoomRepository, IImageRepository imageRepository)
    {
        _gameRoomRepository = gameRoomRepository;
        _imageRepository = imageRepository;
    }

    public async ValueTask<Unit> Handle(VoteCardCommand command, CancellationToken ct = default)
    {
        var gameRoom = await _gameRoomRepository.GetBy(command.GameRoomId, ct)
            ?? throw new GameRoomNotFoundException(command.GameRoomId);

        gameRoom.VoteCard(command.VotingPlayerId, command.SubmittedCardId);
        
        return Unit.Value;
    }
}