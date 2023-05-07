using Guexit.Game.Application.Commands;
using Guexit.Game.Application.Exceptions;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Application.CommandHandlers;

public sealed class SubmitStoryTellerCardStoryCommandHandler : CommandHandler<SubmitStoryTellerCardStoryCommand>
{
    private readonly IGameRoomRepository _gameRoomRepository;

    public SubmitStoryTellerCardStoryCommandHandler(IUnitOfWork unitOfWork, IGameRoomRepository gameRoomRepository) : base(unitOfWork)
    {
        _gameRoomRepository = gameRoomRepository;
    }

    protected override async ValueTask Process(SubmitStoryTellerCardStoryCommand command, CancellationToken ct)
    {
        var gameRoom = await _gameRoomRepository.GetBy(command.GameRoomId, ct) 
            ?? throw new GameRoomNotFoundException(command.GameRoomId);
        
        gameRoom.SubmitCardStory(command.PlayerId, command.CardId, command.Story);
    }
}
