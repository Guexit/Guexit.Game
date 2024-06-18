namespace Guexit.Game.ReadModels;

public sealed class PaginatedCollection<T>
{
    public T[] Items { get; }
    public int TotalItemCount { get; }
    public int PageSize { get; }
    public int PageNumber { get; }
    public int TotalPages { get; }

    public PaginatedCollection(T[] items, int totalItemCount, int pageSize, int pageNumber)
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentOutOfRangeException.ThrowIfNegative(totalItemCount);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(pageSize);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(pageNumber);
        
        Items = items;
        TotalItemCount = totalItemCount;
        PageSize = pageSize;
        PageNumber = pageNumber;
        TotalPages = (int)Math.Ceiling((double)totalItemCount / pageSize);
    }
}