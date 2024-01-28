using Guexit.Game.Application.Commands;
using Guexit.Game.Application.Exceptions;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Mediator;

namespace Guexit.Game.Application.CommandHandlers;

public sealed class ChangePlayerNicknameCommandHandler : ICommandHandler<ChangePlayerNicknameCommand>
{
    private readonly IPlayerRepository _playerRepository;

    public ChangePlayerNicknameCommandHandler(IPlayerRepository playerRepository)
    {
        _playerRepository = playerRepository;
    }

    public async ValueTask<Unit> Handle(ChangePlayerNicknameCommand command, CancellationToken ct = default)
    {
        var player = await _playerRepository.GetBy(command.PlayerId, ct);
        if (player is null)
            throw new PlayerNotFoundException(command.PlayerId);
        
        player.ChangeNickname(command.Nickname);
        
        return Unit.Value;
    }
}