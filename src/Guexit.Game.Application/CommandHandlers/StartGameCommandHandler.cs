using Guexit.Game.Application.Commands;
using Guexit.Game.Application.Exceptions;
using Guexit.Game.Domain.Exceptions;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.ImageAggregate;

namespace Guexit.Game.Application.CommandHandlers;

public sealed class StartGameCommandHandler : CommandHandler<StartGameCommand>
{
    private readonly IGameRoomRepository _gameRoomRepository;
    private readonly IImageRepository _imageRepository;

    public StartGameCommandHandler(IUnitOfWork unitOfWork, IGameRoomRepository gameRoomRepository, IImageRepository imageRepository) : base(unitOfWork)
    {
        _gameRoomRepository = gameRoomRepository;
        _imageRepository = imageRepository;
    }

    protected override async ValueTask Process(StartGameCommand command, CancellationToken ct)
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
    }
}
