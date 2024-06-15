namespace Guexit.Game.Component.IntegrationTests.Contracts;

public sealed class PaginatedCollection<T>
{
    public T[] Items { get; init; } = null!;
    public int TotalItemCount { get; init; }
    public int PageSize { get; init; }
    public int PageNumber { get; init; }
    public int TotalPages { get; init; }
}
