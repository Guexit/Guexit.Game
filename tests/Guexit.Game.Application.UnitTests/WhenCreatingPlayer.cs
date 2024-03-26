using Guexit.Game.Application.Services;
using Guexit.Game.Domain.Exceptions;
using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Application.UnitTests;

public sealed class WhenCreatingPlayer
{
    private readonly IPlayerRepository _playerRepository;
    private readonly PlayerManagementService _service;

    public WhenCreatingPlayer()
    {
        _playerRepository = new FakeInMemoryPlayerRepository();
        _service = new PlayerManagementService(_playerRepository);
    }

    [Fact]
    public async Task IsCreated()
    {
        var playerId = new PlayerId(Guid.NewGuid().ToString());
        var username = "batman";
        var nickname = "batman";

        await _service.CreatePlayer(playerId, username);

        var player = await _playerRepository.GetBy(playerId);
        player.Should().NotBeNull();
        player!.Id.Should().Be(playerId);
        player.Username.Should().Be(username);
        player.Nickname.Value.Should().Be(nickname);
    }

    [Theory]
    [InlineData("pablocom96@gmail.com", "pablocom96")]
    [InlineData("pab.locom96@gmail.com", "pablocom96")]
    [InlineData("ugar-ay_@gmail.com", "ugaray")]
    [InlineData("pab.l-ocom96+123@gmail.com", "pablocom96123")]
    public async Task InitializesNickNameBasedOnUsername(string username, string expectedNickName)
    {
        var playerId = new PlayerId(Guid.NewGuid().ToString());
        
        await _service.CreatePlayer(playerId, username);
        
        var player = await _playerRepository.GetBy(playerId);
        player.Should().NotBeNull();
        player!.Nickname.Value.Should().Be(expectedNickName);
    }
    
    [Theory]
    [InlineData("@pablocom96@gmail.com", "@pablocom96@gmail.com")]
    [InlineData("._@gmail.com", "._@gmail.com")]
    [InlineData("._+_-_", "._+_-_")]
    public async Task IfNickNameCalculatedIsEmptyItInitializesToItsEmail(string username, string expectedNickName)
    {
        var playerId = new PlayerId(Guid.NewGuid().ToString());
        
        await _service.CreatePlayer(playerId, username);
        
        var player = await _playerRepository.GetBy(playerId);
        player.Should().NotBeNull();
        player!.Nickname.Value.Should().Be(expectedNickName);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task ThrowsInvalidArgumentExceptionIfUsernameIsEmpty(string username)
    {
        var playerId = new PlayerId(Guid.NewGuid().ToString());

        var act = async () => await _service.CreatePlayer(playerId, username);

        await act.Should().ThrowAsync<EmptyUsernameException>();
    }
}
