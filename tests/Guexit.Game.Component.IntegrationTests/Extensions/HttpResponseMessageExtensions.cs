using System.Net;

namespace Guexit.Game.Component.IntegrationTests.Extensions;

public static class HttpResponseMessageExtensions
{
    public static async Task ShouldHaveSuccessStatusCode(this HttpResponseMessage responseMessage)
    {
        responseMessage.StatusCode.Should().Be(HttpStatusCode.OK, 
            because: await responseMessage.Content.ReadAsStringAsync());
    }
}
