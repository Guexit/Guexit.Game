using System.Net;

namespace Guexit.Game.Component.IntegrationTests.Extensions;

public static class HttpResponseMessageExtensions
{
    public static async Task ShouldHaveSuccessStatusCode(this HttpResponseMessage responseMessage)
    {
        responseMessage.Should().NotBeNull();
        responseMessage.IsSuccessStatusCode.Should().BeTrue(
            because: await responseMessage.Content.ReadAsStringAsync());
    }

    public static async Task ShouldHaveStatusCode(this HttpResponseMessage responseMessage, HttpStatusCode expectedStatusCode)
    {
        responseMessage.Should().NotBeNull();
        responseMessage.StatusCode.Should().Be(expectedStatusCode,
            because: await responseMessage.Content.ReadAsStringAsync());
    }
}
