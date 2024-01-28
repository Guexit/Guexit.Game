using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.WebApi;

namespace Guexit.Game.Component.IntegrationTests.Builders;

public static class HttpRequestMessageExtensions
{
    public static void AddPlayerIdHeader(this HttpRequestMessage request, PlayerId playerId)
    {
        request.Headers.Add(GuexitHttpHeaders.AuthenticatedUserId, playerId.Value);
    }
}
