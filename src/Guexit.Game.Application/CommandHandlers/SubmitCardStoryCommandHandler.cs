using Guexit.Game.Application.Commands;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Application.CommandHandlers;

public sealed class SubmitCardStoryCommandHandler : CommandHandler<SubmitCardStoryCommand>
{
    private readonly IPlayerRepository _playerRepository;
    private readonly IGameRoomRepository _gameRoomRepository;

    public SubmitCardStoryCommandHandler(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }

    protected override async ValueTask Process(SubmitCardStoryCommand command, CancellationToken ct)
    {
        
    }
}
