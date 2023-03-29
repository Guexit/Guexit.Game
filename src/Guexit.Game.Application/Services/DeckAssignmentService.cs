using Guexit.Game.Application.CardAssigment;
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
    private readonly ILogicalShardDistributedLock _logicalShardDistributedLock;
    private readonly ILogicalShardProvider _logicalShardProvider;
    private readonly IImageRepository _imageRepository;
    private readonly IGameRoomRepository _gameRoomRepository;

    public DeckAssignmentService(
        ILogicalShardDistributedLock logicalShardDistributedLock,
        ILogicalShardProvider logicalShardProvider,
        IImageRepository _imageRepository,
        IGameRoomRepository _gameRoomRepository)
    {
        _logicalShardDistributedLock = logicalShardDistributedLock;
        _logicalShardProvider = logicalShardProvider;
        this._imageRepository = _imageRepository;
        this._gameRoomRepository = _gameRoomRepository;
    }

    public async Task AssignDeck(GameRoomId gameRoomId, CancellationToken cancellationToken = default)
    {
        var logicalShard = _logicalShardProvider.GetLogicalShard();
        try
        {
            await _logicalShardDistributedLock.Acquire(logicalShard, cancellationToken);
            await AssignDeckToGameRoom(gameRoomId, logicalShard, cancellationToken);
        }
        finally
        {
            await _logicalShardDistributedLock.Release(logicalShard, cancellationToken);
        }
    }

    private async Task AssignDeckToGameRoom(GameRoomId gameRoomId, int logicalShard, CancellationToken cancellationToken)
    {
        var gameRoom = await _gameRoomRepository.GetBy(gameRoomId, cancellationToken);
        if (gameRoom is null)
            throw new GameRoomNotFoundException(gameRoomId);

        var count = await _imageRepository.CountAvailableImages(logicalShard, cancellationToken);
        if (count < gameRoom.GetRequiredNumberOfCardsInDeck())
            throw new InsufficientImagesToAssignDeckException(count, gameRoomId);

        var images = await _imageRepository.GetAvailableImages(gameRoom.GetRequiredNumberOfCardsInDeck(), logicalShard, cancellationToken);
        var cards = images.Select(x => new Card(Guid.NewGuid(), x.Url)).ToArray();
        gameRoom.AssignDeck(cards);
    }
}
