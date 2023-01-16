using TryGuessIt.Game.Domain.Model.PlayerAggregate;
using TryGuessIt.Game.WebApi;

namespace TryGuessIt.Game.Component.IntegrationTests.Builders;

public static class HttpRequestMessageExtensions
{
    public static void AddPlayerIdHeader(this HttpRequestMessage request, PlayerId playerId)
    {
        request.Headers.Add(TryGuessItHttpHeaders.UserId, playerId.Value);
    }
}
