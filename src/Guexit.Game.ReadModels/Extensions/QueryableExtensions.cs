using Microsoft.EntityFrameworkCore;

namespace Guexit.Game.ReadModels.Extensions;

public static class QueryableExtensions
{
    public static async Task<PaginatedCollection<T>> PaginateAsync<T>(this IQueryable<T> queryable, PaginationSettings paginationSettings, CancellationToken ct = default)
    {
        var items = await queryable.Skip(paginationSettings.PageSize * (paginationSettings.PageNumber - 1))
            .Take(paginationSettings.PageSize)
            .ToArrayAsync(ct);

        var totalAvailableGameRooms = await queryable.CountAsync(ct);

        return new(items, totalAvailableGameRooms, paginationSettings.PageSize, paginationSettings.PageNumber);
    }
}