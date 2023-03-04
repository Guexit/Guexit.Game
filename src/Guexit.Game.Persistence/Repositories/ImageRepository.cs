using Guexit.Game.Domain.Model.ImageAggregate;
using Microsoft.EntityFrameworkCore;

namespace TryGuessIt.Game.Persistence.Repositories;

public sealed class ImageRepository : IImageRepository
{
    private readonly GameDbContext _dbContext;

    public ImageRepository(GameDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask Add(Image player, CancellationToken ct = default)
    {
        await _dbContext.AddAsync(player, ct);
    }

    public async Task<Image?> GetBy(ImageId id, CancellationToken ct = default)
    {
        return await _dbContext.Images.FirstOrDefaultAsync(x => x.Id == id, ct);
    }
}
