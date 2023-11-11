using Guexit.Game.Domain.Model.ImageAggregate;
using Guexit.Game.Persistence.Repositories;
using Xunit.Abstractions;

namespace Guexit.Game.Persistence.IntegrationTests;

public sealed class WhenSavingImage : DatabaseMappingIntegrationTest
{
    public WhenSavingImage(IntegrationTestFixture fixture, ITestOutputHelper testOutput) : base(fixture, testOutput)
    {
    }

    [Fact]
    public async Task IsPersisted()
    {
        var imageId = new ImageId(Guid.NewGuid());
        var url = new Uri("https://pablocompany.com/images/2");
        var createdAt = new DateTimeOffset(2023, 3, 4, 9, 38, 5, TimeSpan.Zero);
        var repository = new ImageRepository(DbContext);

        await repository.Add(new Image(imageId, url, createdAt));
        await SaveChanges();

        var image = await repository.GetBy(imageId);
        image.Should().NotBeNull();
        image!.Id.Should().Be(imageId);
        image.Url.Should().Be(url);
        image.CreatedAt.Should().Be(createdAt);
    }
}
