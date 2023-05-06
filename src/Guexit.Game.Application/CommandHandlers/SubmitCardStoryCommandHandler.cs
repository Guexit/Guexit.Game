using Guexit.Game.Application.Commands;
using Guexit.Game.Application.Exceptions;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Application.CommandHandlers;

public sealed class SubmitCardStoryCommandHandler : CommandHandler<SubmitCardStoryCommand>
{
    private readonly IGameRoomRepository _gameRoomRepository;

    public SubmitCardStoryCommandHandler(IUnitOfWork unitOfWork, IGameRoomRepository gameRoomRepository) : base(unitOfWork)
    {
        _gameRoomRepository = gameRoomRepository;
    }

    protected override async ValueTask Process(SubmitCardStoryCommand command, CancellationToken ct)
    {
        var gameRoom = await _gameRoomRepository.GetBy(command.GameRoomId, ct) 
            ?? throw new GameRoomNotFoundException(command.GameRoomId);
        
        gameRoom.SubmitCardStory(command.PlayerId, command.CardId, command.Story);
    }
}
