using Guexit.Game.Domain.Model.PlayerAggregate;
using Microsoft.Extensions.DependencyInjection;
using TryGuessIt.Game.Persistence;
using TryGuessIt.IdentityProvider.Messages;

namespace TryGuessIt.Game.Component.IntegrationTests;

public sealed class WhenReceivingUserCreated : ComponentTestBase
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public WhenReceivingUserCreated(GameWebApplicationFactory factory) : base(factory)
    {
        _serviceScopeFactory = WebApplicationFactory.Services.GetRequiredService<IServiceScopeFactory>();
    }

    [Fact]
    public async Task PlayerIsCreated()
    {
        var userCreatedEvent = new UserCreated(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

        await PublishAndWaitUntilConsumed(userCreatedEvent);

        await AssertPlayerWasCreated(userCreatedEvent);
    }

    private async Task AssertPlayerWasCreated(UserCreated userCreatedEvent)
    {
        await using var scope = _serviceScopeFactory.CreateAsyncScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<GameDbContext>();

        var player = await dbContext.Players.FindAsync(new PlayerId(userCreatedEvent.Id));
        player.Should().NotBeNull();
        player!.Id.Should().NotBeNull(userCreatedEvent.Id);
        player.Username.Should().NotBeNull(userCreatedEvent.Username);
    }
}