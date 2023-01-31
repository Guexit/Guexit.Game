using Guexit.Game.Application.Commands;
using Guexit.Game.Application.Exceptions;
using Guexit.Game.Domain;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Mediator;

namespace Guexit.Game.Application.CommandHandlers;

public sealed class JoinGameRoomCommandHandler : CommandHandler<JoinGameRoomCommand, Unit>
{
    private readonly IPlayerRepository _playerRepository;
    private readonly IGameRoomRepository _gameRoomRepository;
    private readonly IDomainEventPublisher _domainEventPublisher;

    public JoinGameRoomCommandHandler(IUnitOfWork unitOfWork, IPlayerRepository playerRepository, IGameRoomRepository gameRoomRepository,
        IDomainEventPublisher domainEventPublisher) 
        : base(unitOfWork)
    {
        _playerRepository = playerRepository;
        _gameRoomRepository = gameRoomRepository;
        _domainEventPublisher = domainEventPublisher;
    }

    protected override async ValueTask<Unit> Process(JoinGameRoomCommand command, CancellationToken ct)
    {
        var player = await _playerRepository.GetBy(command.PlayerId, ct);
        if (player is null)
            throw new PlayerNotFoundException(command.PlayerId);

        var gameRoom = await _gameRoomRepository.GetBy(command.GameRoomId, ct);
        if (gameRoom is null)
            throw new GameRoomNotFoundException(command.GameRoomId);

        gameRoom.Join(player.Id);

        await _domainEventPublisher.Publish(gameRoom.DomainEvents, ct);

        return Unit.Value;
    }
}
