using Guexit.Game.Domain.Model.GameRoomAggregate;

namespace Guexit.Game.Application.Services;

public interface IDeckAssignmentService
{
    Task AssignDeck(GameRoomId gameRoomId, CancellationToken cancellationToken = default);
}

public class DeckAssignmentService : IDeckAssignmentService
{
    public Task AssignDeck(GameRoomId gameRoomId, CancellationToken cancellationToken = default)
    {
        //return Task.FromException(new NotImplementedException());        //return Task.FromException(new NotImplementedException());
        return Task.CompletedTask;
    }
}
