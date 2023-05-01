using Guexit.Game.Application.Exceptions;
using Guexit.Game.Domain.Exceptions;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.ImageAggregate;

namespace Guexit.Game.Application.Services;

public interface IDeckAssignmentService
{
    Task AssignDeck(GameRoomId gameRoomId, CancellationToken cancellationToken = default);
}

public sealed class DeckAssignmentService : IDeckAssignmentService
{
    private readonly IImageRepository _imageRepository;
    private readonly IGameRoomRepository _gameRoomRepository;

    public DeckAssignmentService(
        IImageRepository _imageRepository,
        IGameRoomRepository _gameRoomRepository)
    {
        this._imageRepository = _imageRepository;
        this._gameRoomRepository = _gameRoomRepository;
    }

    public async Task AssignDeck(GameRoomId gameRoomId, CancellationToken cancellationToken = default)
    {
        var gameRoom = await _gameRoomRepository.GetBy(gameRoomId, cancellationToken);
        if (gameRoom is null)
            throw new GameRoomNotFoundException(gameRoomId);

        var images = await _imageRepository.GetAvailableImages(gameRoom.GetRequiredNumberOfCardsInDeck(), cancellationToken);
        var cards = images.Select(x => new Card(Guid.NewGuid(), x.Url)).ToArray();
        
        gameRoom.AssignDeck(cards);

        foreach (var image in images)
            image.AssignToGame(gameRoom.Id);
    }
}
