using Guexit.Game.Domain.Model.ImageAggregate;
using Guexit.Game.Messages;
using Guexit.Game.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

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

        await using var scope = WebApplicationFactory.Services.CreateAsyncScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<GameDbContext>();

        var image = await dbContext.Images.SingleAsync(x => x.Url == new Uri(imageUrl));
        image.Url.Should().Be(new Uri(imageUrl));
    }
}