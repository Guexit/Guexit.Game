using Guexit.Game.Application.CommandHandlers;
using Guexit.Game.Application.Commands;
using Guexit.Game.Application.Exceptions;
using Guexit.Game.Domain.Exceptions;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.Tests.Common.Builders;

namespace Guexit.Game.Application.UnitTests;

public sealed class WhenHandlingChangePlayerNickname
{
    private readonly FakeInMemoryPlayerRepository _playerRepository;
    private readonly ChangePlayerNicknameCommandHandler _commandHandler;

    public WhenHandlingChangePlayerNickname()
    {
        _playerRepository = new FakeInMemoryPlayerRepository();
        _commandHandler = new ChangePlayerNicknameCommandHandler(_playerRepository);
    }
    
    [Fact]
    public async Task ThrowsPlayerNotFoundException()
    {
        var command = new ChangePlayerNicknameCommand("nonExistingPlayerId", "New nickname");

        var action = async () => await _commandHandler.Handle(command);

        await action.Should().ThrowAsync<PlayerNotFoundException>();
    }
    
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void ThrowsInvalidNicknameExceptionIfItsNullOrWhitespace(string nickname)
    {
        var action = () => new ChangePlayerNicknameCommand("playerId", nickname);

        action.Should().Throw<InvalidNicknameException>()
            .WithMessage("Nickname is invalid because it's empty or composed only by whitespaces");
    }
    
    [Fact]
    public async Task NicknameIsChanged()
    {
        var newNickname = "Alexander9_jfed3bnRTjK08.Smith";
        var playerId = new PlayerId("d3fd8605-5e5f-4ddd-9d53-f7f5deb560d0");
        var username = "oldnickname@guexit.com";
        await _playerRepository.Add(new PlayerBuilder()
            .WithId(playerId)
            .WithUsername(username)
            .Build());

        await _commandHandler.Handle(new ChangePlayerNicknameCommand(playerId.Value, newNickname));
        
        var player = await _playerRepository.GetBy(playerId);
        player!.Nickname.Should().NotBeNull();
        player.Nickname.Value.Should().Be(newNickname);
    }
}