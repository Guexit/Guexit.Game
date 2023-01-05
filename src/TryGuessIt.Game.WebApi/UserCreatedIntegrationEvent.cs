namespace TryGuessIt.IdentityProvider.WebApi.Events;

public sealed class UserCreatedIntegrationEvent
{
    public string Id { get; }

    public UserCreatedIntegrationEvent(string id)
    {
        Id = id;
    }
}
