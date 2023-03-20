using Guexit.Game.ExternalMessageHandlers;
using Guexit.Game.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Guexit.Game.Component.IntegrationTests;

public sealed class WhenReceivingImageGenerated : ComponentTestBase
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public WhenReceivingImageGenerated(GameWebApplicationFactory factory) : base(factory)
    {
        _serviceScopeFactory = WebApplicationFactory.Services.GetRequiredService<IServiceScopeFactory>();
    }

    [Fact]
    public async Task ImageIsAdded()
    {
        var imageUrl = "https://pablocompany.com";
        var imageGenerated = new ImageGenerated(imageUrl);

        await ConsumeMessage(imageGenerated);

        await AssertImageWasAdded(imageUrl);
    }

    private async Task AssertImageWasAdded(string imageUrl)
    {
        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<GameDbContext>();

        var images = await dbContext.Images.ToArrayAsync();
        images.Should().HaveCount(1);
        images[0].Url.Should().Be(imageUrl);
    }
}