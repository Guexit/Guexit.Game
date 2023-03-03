using Guexit.Game.Domain.Model.ImageAggregate;

namespace TryGuessIt.Game.Persistence.Repositories;

public sealed class ImageRepository : IImageRepository
{
    private readonly GameDbContext _dbContext;

    public ImageRepository(GameDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public ValueTask Add(Image player, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<Image?> GetBy(ImageId id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}
