using NSubstitute;
using TryGuessIt.Game.Application.CommandHandlers;
using TryGuessIt.Game.Application.Commands;
using TryGuessIt.Game.Domain.Model.PlayerAggregate;

namespace TryGuessIt.Game.Application.UnitTests;

public sealed class WhenHandlingCreatePlayerCommand
{
    private readonly IPlayerRepository _playerRepository;
    private readonly CreatePlayerCommandHandler _commandHandler;

    public WhenHandlingCreatePlayerCommand()
    {
        _playerRepository = new FakeInMemoryPlayerRepository();
        _commandHandler = new CreatePlayerCommandHandler(
            Substitute.For<IUnitOfWork>(), 
            new PlayerManagementService(_playerRepository)
        );
    }

    [Fact]
    public async Task IsCreated()
    {
        var playerId = Guid.NewGuid().ToString();
        var username = Guid.NewGuid().ToString();

        await _commandHandler.Handle(new CreatePlayerCommand(playerId, username));

        var player = await _playerRepository.GetById(playerId);
        player.Should().NotBeNull();
        player!.Id.Should().Be(playerId);
        player.Username.Should().Be(username);
    }
}
