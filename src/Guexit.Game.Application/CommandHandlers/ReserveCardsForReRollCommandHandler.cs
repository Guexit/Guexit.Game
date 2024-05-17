using Guexit.Game.Application.Commands;
using Guexit.Game.Application.Exceptions;
using Guexit.Game.Domain.Exceptions;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.ImageAggregate;
using Mediator;

namespace Guexit.Game.Application.CommandHandlers;

public sealed class ReserveCardsForReRollCommandHandler : ICommandHandler<ReserveCardsForReRollCommand>
{
    private readonly IGameRoomRepository _gameRoomRepository;
    private readonly IImageRepository _imageRepository;

    public ReserveCardsForReRollCommandHandler(IGameRoomRepository gameRoomRepository, IImageRepository imageRepository)
    {
        _gameRoomRepository = gameRoomRepository;
        _imageRepository = imageRepository;

    }

    public async ValueTask<Unit> Handle(ReserveCardsForReRollCommand command, CancellationToken ct = default)
    {
        var gameRoom = await _gameRoomRepository.GetBy(command.GameRoomId, ct) 
            ?? throw new GameRoomNotFoundException(command.GameRoomId);

        var images = await _imageRepository.GetAvailableImages(CardReRoll.RequiredReservedCardsSize, ct);

        if (images.Length < CardReRoll.RequiredReservedCardsSize)
            throw new InsufficientCardsAvailableToReRollException(images.Length, command.GameRoomId, command.PlayerId);

        var cards = images.Select(x => new Card(Guid.NewGuid(), x.Url)).ToArray();
        gameRoom.ReserveCardsForReRoll(command.PlayerId, cards);

        foreach (var image in images)
            image.AssignTo(gameRoom.Id);

        return Unit.Value;
    }
}
