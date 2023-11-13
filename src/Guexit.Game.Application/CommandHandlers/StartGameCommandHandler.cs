using Guexit.Game.Application.Commands;
using Guexit.Game.Application.Exceptions;
using Guexit.Game.Domain.Exceptions;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.ImageAggregate;
using Mediator;

namespace Guexit.Game.Application.CommandHandlers;

public sealed class StartGameCommandHandler : ICommandHandler<StartGameCommand>
{
    private readonly IGameRoomRepository _gameRoomRepository;
    private readonly IImageRepository _imageRepository;

    public StartGameCommandHandler(IGameRoomRepository gameRoomRepository, IImageRepository imageRepository)
    {
        _gameRoomRepository = gameRoomRepository;
        _imageRepository = imageRepository;
    }

    public async ValueTask<Unit> Handle(StartGameCommand command, CancellationToken ct = default)
    {
        var gameRoom = await _gameRoomRepository.GetBy(command.GameRoomId, ct);
        if (gameRoom is null)
            throw new GameRoomNotFoundException(command.GameRoomId);
        
        var images = await _imageRepository.GetAvailableImages(gameRoom.GetRequiredNumberOfCardsInDeck(), ct);
        var cards = images.Select(x => new Card(Guid.NewGuid(), x.Url)).ToArray();
        
        gameRoom.AssignDeck(cards);
        gameRoom.Start(command.PlayerId);
        
        foreach (var image in images)
            image.AssignTo(gameRoom.Id);
        
        return Unit.Value;
    }
}
