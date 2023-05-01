using Guexit.Game.Application.CommandHandlers;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Application.UnitTests;

public sealed class WhenHandlingSubmitCardStoryCommand
{
    private readonly IPlayerRepository _playerRepository;
    private readonly IGameRoomRepository _gameRoomRepository;

    private readonly SubmitCardStoryCommandHandler _commandHandler;

    public WhenHandlingSubmitCardStoryCommand()
    {
        _commandHandler = new SubmitCardStoryCommandHandler(
            Substitute.For<IUnitOfWork>()
            );
    }

    /**
     * 1. Error if player is not the story teller
     * 2. Error if game is not in progress?? -> same than point 4
     * 3. Error if game is not found
     * 4. Error if player is not in game room?? -> Maybe cross cut concern should be hidden and executed this check under the hood.
     * 5. If already submitted a story, domain error
     * 6. Story is sbmitted and domain events were raised, layer published as notification to the front
     */
    [Fact]
    public async Task ThrowsGameRoomNotFoundExceptionIfDoesNotExist()
    {

    }
}
