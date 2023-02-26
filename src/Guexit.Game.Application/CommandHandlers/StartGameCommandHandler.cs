using Guexit.Game.Application.Commands;
using Mediator;

namespace Guexit.Game.Application.CommandHandlers;

public sealed class StartGameCommandHandler : CommandHandler<StartGameCommand, Unit>
{
    public StartGameCommandHandler(IUnitOfWork unitOfWork, Domain.Model.GameRoomAggregate.IGameRoomRepository _gameRoomRepository) : base(unitOfWork)
    {
    }

    protected override ValueTask<Unit> Process(StartGameCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
