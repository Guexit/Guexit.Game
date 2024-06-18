using Guexit.Game.ReadModels.ReadModels;
using Guexit.Game.ReadModels.ReadOnlyRepositories;

namespace Guexit.Game.ReadModels.QueryHandlers;

public sealed class AvailableGameRoomsQuery : IQuery<PaginatedCollection<AvailableGameRoomReadModel>>
{
    public PaginationSettings PaginationSettings { get; }

    public AvailableGameRoomsQuery(PaginationSettings paginationSettings)
    {
        PaginationSettings = paginationSettings;
    }
}

public sealed class AvailableGameRoomsQueryHandler : IQueryHandler<AvailableGameRoomsQuery, PaginatedCollection<AvailableGameRoomReadModel>>
{
    private readonly ReadOnlyGameRoomRepository _gameRoomRepository;

    public AvailableGameRoomsQueryHandler(ReadOnlyGameRoomRepository gameRoomRepository)
    {
        _gameRoomRepository = gameRoomRepository;
    }

    public async ValueTask<PaginatedCollection<AvailableGameRoomReadModel>> Handle(AvailableGameRoomsQuery query, CancellationToken ct)
    {
        var pagedGameRooms = await _gameRoomRepository.GetAvailable(query.PaginationSettings, ct);

        var readModels = pagedGameRooms.Items.Select(g => new AvailableGameRoomReadModel
        {
            GameRoomId = g.Id,
            RequiredMinPlayers = g.RequiredMinPlayers.Count,
            CreatedAt = g.CreatedAt,
            CurrentPlayerCount = g.GetPlayerCount()
        }).ToArray();
        
        return new PaginatedCollection<AvailableGameRoomReadModel>(
            readModels, 
            pagedGameRooms.TotalItemCount, 
            pagedGameRooms.PageSize, 
            pagedGameRooms.PageNumber
        );
    }
}