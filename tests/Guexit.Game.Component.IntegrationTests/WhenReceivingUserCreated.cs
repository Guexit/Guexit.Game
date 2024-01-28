using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.IdentityProvider.Messages;
using Microsoft.Extensions.DependencyInjection;

namespace Guexit.Game.Component.IntegrationTests;

public sealed class WhenReceivingUserCreated : ComponentTest
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public WhenReceivingUserCreated(GameWebApplicationFactory factory) : base(factory)
    {
        _serviceScopeFactory = WebApplicationFactory.Services.GetRequiredService<IServiceScopeFactory>();
    }

    [Fact]
    public async Task PlayerIsCreated()
    {
        var userCreatedEvent = new UserCreated(Guid.NewGuid().ToString(), "newplayer@guexit.com");

        await ConsumeMessage(userCreatedEvent);

        await AssertPlayerWasCreated(userCreatedEvent);
    }

    private async Task AssertPlayerWasCreated(UserCreated userCreatedEvent)
    {
        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        var playerRepository = scope.ServiceProvider.GetRequiredService<IPlayerRepository>();

        var player = await playerRepository.GetBy(new PlayerId(userCreatedEvent.Id));
        player.Should().NotBeNull();
        player!.Id.Should().NotBeNull(userCreatedEvent.Id);
        player.Username.Should().NotBeNull(userCreatedEvent.Username);
        player.Nickname.Should().Be(Nickname.From(userCreatedEvent.Username));
    }
}
