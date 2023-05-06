using Guexit.Game.Domain.Model.ImageAggregate;
using Guexit.Game.ExternalMessageHandlers;

namespace Guexit.Game.Component.IntegrationTests;

public sealed class WhenReceivingImageGenerated : ComponentTest
{
    public WhenReceivingImageGenerated(GameWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task ImageIsAdded()
    {
        var imageUrl = "https://pablocompany.com";
        var imageGenerated = new ImageGenerated(imageUrl);

        await ConsumeMessage(imageGenerated);

        var image = GetSingle<Image>(x => x.Url == new Uri(imageUrl));
        image.Url.Should().Be(new Uri(imageUrl));
    }
}