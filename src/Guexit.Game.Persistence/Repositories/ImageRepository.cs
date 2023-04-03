using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.ImageAggregate;
using Microsoft.EntityFrameworkCore;

namespace Guexit.Game.Persistence.Repositories;

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

    public async ValueTask AddRange(IEnumerable<Image> images, CancellationToken ct = default)
    {
        await _dbContext.Images.AddRangeAsync(images, ct);
    }

    public async Task<int> CountAvailableImages(int logicalShard, CancellationToken ct = default)
    {
        return await _dbContext.Images.CountAsync(x => x.GameRoomId == GameRoomId.Empty, ct);
    }

    public async Task<Image[]> GetAvailableImages(int amount, int logicalShard, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Images
            .Take(amount)
            .Where(x => x.GameRoomId == GameRoomId.Empty && x.LogicalShard == logicalShard)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<Image?> GetBy(ImageId id, CancellationToken ct = default)
    {
        return await _dbContext.Images.FirstOrDefaultAsync(x => x.Id == id, ct);
    }
}
