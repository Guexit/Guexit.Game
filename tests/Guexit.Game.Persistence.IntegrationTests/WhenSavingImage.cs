using Guexit.Game.Domain.Model.ImageAggregate;
using Guexit.Game.Persistence.Repositories;
using Guexit.Game.Tests.Common.Builders;
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
        var tags = new[] { new Tag("style:mange"), new Tag("model:turbo_v1") };
        var repository = new ImageRepository(DbContext);

        await repository.Add(new ImageBuilder()
            .WithId(imageId)
            .WithUrl(url)
            .WithCreatedAt(createdAt)
            .WithTags(tags)
            .Build());
        await SaveChanges();

        var image = await repository.GetBy(imageId);
        image.Should().NotBeNull();
        image!.Id.Should().Be(imageId);
        image.Url.Should().Be(url);
        image.CreatedAt.Should().Be(createdAt);
        image.Tags.Should().HaveCount(2);
        image.Tags.Should().BeEquivalentTo(tags);
    }
}
