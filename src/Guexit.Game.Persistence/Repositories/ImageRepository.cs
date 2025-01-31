﻿using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.ImageAggregate;
using Microsoft.EntityFrameworkCore;

namespace Guexit.Game.Persistence.Repositories;

public sealed class ImageRepository : IImageRepository
{
    private readonly GameDbContext _dbContext;

    public ImageRepository(GameDbContext dbContext) => _dbContext = dbContext;

    public async ValueTask Add(Image player, CancellationToken ct = default) 
        => await _dbContext.AddAsync(player, ct);

    public async ValueTask AddRange(IEnumerable<Image> images, CancellationToken ct = default) 
        => await _dbContext.Images.AddRangeAsync(images, ct);

    public async Task<int> CountAvailableImages(CancellationToken ct = default) 
        => await _dbContext.Images.CountAsync(x => x.GameRoomId == GameRoomId.Empty, ct);

    public async Task<Image[]> GetAvailableImages(int limit, CancellationToken ct = default)
    {
        var subsetSize = limit < 10 ? 1000 : limit * 10;
        FormattableString subsetQuery = $"""
             WITH subset AS (
                 SELECT *, xmin FROM public."Images" i
                 WHERE i."GameRoomId" = {GameRoomId.Empty.Value}
                 LIMIT {subsetSize}
             )
             SELECT * FROM subset
             ORDER BY RANDOM()
             LIMIT {limit}
             FOR UPDATE SKIP LOCKED
             """;

        var images = await _dbContext.Images.FromSqlInterpolated(subsetQuery).ToArrayAsync(ct);
        return images;
    }

    public async Task<Image[]> GetBy(IEnumerable<Uri> imageUrls, CancellationToken ct = default)
    {
        var images = await _dbContext.Images.Where(x => imageUrls.Contains(x.Url)).ToArrayAsync(ct);
        return images;
    }

    public async Task<Image?> GetBy(ImageId id, CancellationToken ct = default) 
        => await _dbContext.Images.FirstOrDefaultAsync(x => x.Id == id, ct);
}
