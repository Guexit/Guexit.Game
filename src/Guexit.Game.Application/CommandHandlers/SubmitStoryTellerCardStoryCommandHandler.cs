using Guexit.Game.Application.Commands;
using Guexit.Game.Application.Exceptions;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Mediator;

namespace Guexit.Game.Application.CommandHandlers;

public sealed class SubmitStoryTellerCardStoryCommandHandler : ICommandHandler<SubmitStoryTellerCardStoryCommand>
{
    private readonly IGameRoomRepository _gameRoomRepository;

    public SubmitStoryTellerCardStoryCommandHandler(IGameRoomRepository gameRoomRepository)
    {
        _gameRoomRepository = gameRoomRepository;
    }

    public async ValueTask<Unit> Handle(SubmitStoryTellerCardStoryCommand command, CancellationToken ct = default)
    {
        var gameRoom = await _gameRoomRepository.GetBy(command.GameRoomId, ct) 
            ?? throw new GameRoomNotFoundException(command.GameRoomId);
        
        gameRoom.SubmitStory(command.PlayerId, command.CardId, command.Story);

        return Unit.Value;
    }
}
