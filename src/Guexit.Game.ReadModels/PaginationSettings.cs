namespace Guexit.Game.ReadModels;

public sealed class PaginationSettings
{
    public int PageSize { get; }
    public int PageNumber { get; }

    public PaginationSettings(int pageSize, int pageNumber)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(pageSize);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(pageNumber);
        
        PageSize = pageSize;
        PageNumber = pageNumber;
    }
}