using System.Net.Http.Json;
using Guexit.Game.Component.IntegrationTests.Contracts;
using Guexit.Game.Component.IntegrationTests.Extensions;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.ReadModels.ReadModels;
using Guexit.Game.Tests.Common.Builders;

namespace Guexit.Game.Component.IntegrationTests;

public sealed class WhenQueryingAvailableGameRooms : ComponentTest
{
    private static readonly PlayerId AnyPlayerId = new(Guid.NewGuid().ToString());
    private static readonly DateTimeOffset Now = new(2024, 6, 15, 0, 0, 0, TimeSpan.Zero);
    
    public WhenQueryingAvailableGameRooms(GameWebApplicationFactory factory) : base(factory)
    { }
    
    [Fact]
    public async Task ReturnsCurrentlyAvailableGameRooms()
    {
        var gameRoom1 = new GameRoomBuilder()
            .WithIsPublic(true)
            .WithId(Guid.NewGuid())
            .WithCreator("player1")
            .WithPlayersThatJoined("player2", "player3")
            .WithCreatedAt(Now)
            .Build();
        var gameRoom2 = new GameRoomBuilder()
            .WithIsPublic(true)
            .WithId(Guid.NewGuid())
            .WithCreator("player4")
            .WithPlayersThatJoined("player5", "player6", "player7")
            .WithCreatedAt(Now.AddMinutes(-4))
            .Build();
        
        await SaveInRepository(
            gameRoom1,
            gameRoom2,
            new GameRoomBuilder()
                .WithIsPublic(false)
                .WithId(Guid.NewGuid())
                .WithCreator("player8")
                .WithPlayersThatJoined("player9", "player10")
                .Build(),
            GameRoomBuilder.CreateStarted(Guid.NewGuid(), "player8", ["player9", "player10"])
                .WithIsPublic(true)
                .Build()
        );
        
        using var response = await Send(HttpMethod.Get, "game-rooms/available?pageNumber=1&pageSize=5", AnyPlayerId);
        await response.ShouldHaveSuccessStatusCode();
        
        var paginatedReadModels = await response.Content.ReadFromJsonAsync<PaginatedCollection<AvailableGameRoomReadModel>>();
        paginatedReadModels.Should().NotBeNull();
        paginatedReadModels!.PageNumber.Should().Be(1);
        paginatedReadModels.PageSize.Should().Be(5);
        paginatedReadModels.TotalPages.Should().Be(1);
        paginatedReadModels.TotalItemCount.Should().Be(2);
        paginatedReadModels.Items.Should().HaveCount(2);
        
        var room1 = paginatedReadModels.Items.First(x => x.GameRoomId == gameRoom1.Id);
        var room2 = paginatedReadModels.Items.First(x => x.GameRoomId == gameRoom2.Id);

        room1.RequiredMinPlayers.Should().Be(gameRoom1.RequiredMinPlayers.Count);
        room1.CreatedAt.Should().Be(gameRoom1.CreatedAt);
        room1.CurrentPlayerCount.Should().Be(gameRoom1.PlayerCount);
        
        room2.RequiredMinPlayers.Should().Be(gameRoom2.RequiredMinPlayers.Count);
        room2.CreatedAt.Should().Be(gameRoom2.CreatedAt);
        room2.CurrentPlayerCount.Should().Be(gameRoom2.PlayerCount);
    }
    
    [Fact]
    public async Task ExcludesPrivateGameRooms()
    {
        var gameRoom = new GameRoomBuilder()
            .WithIsPublic(true)
            .WithId(Guid.NewGuid())
            .WithCreator("player1")
            .WithPlayersThatJoined("player2", "player3")
            .Build();
        
        await SaveInRepository(
            gameRoom,
            new GameRoomBuilder()
                .WithIsPublic(false)
                .WithId(Guid.NewGuid())
                .WithCreator("player8")
                .WithPlayersThatJoined("player9", "player10")
                .Build()
        );
        
        using var response = await Send(HttpMethod.Get, "game-rooms/available?pageNumber=1&pageSize=5", AnyPlayerId);
        await response.ShouldHaveSuccessStatusCode();
        
        var paginatedReadModels = await response.Content.ReadFromJsonAsync<PaginatedCollection<AvailableGameRoomReadModel>>();
        paginatedReadModels.Should().NotBeNull();
        paginatedReadModels!.Items.Should().HaveCount(1);
        
        var room1 = paginatedReadModels.Items.Single();
        room1.GameRoomId.Should().Be(gameRoom.Id);
    }
    
    [Fact]
    public async Task ExcludesStartedGameRooms()
    {
        var gameRoom = new GameRoomBuilder()
            .WithIsPublic(true)
            .WithId(Guid.NewGuid())
            .WithCreator("player1")
            .WithPlayersThatJoined("player2", "player3")
            .Build();
        
        await SaveInRepository(
            gameRoom,
            GameRoomBuilder.CreateStarted(Guid.NewGuid(), "player8", ["player9", "player10"])
                .WithIsPublic(true)
                .Build()
        );
        
        using var response = await Send(HttpMethod.Get, "game-rooms/available?pageNumber=1&pageSize=5", AnyPlayerId);
        await response.ShouldHaveSuccessStatusCode();
        
        var paginatedReadModels = await response.Content.ReadFromJsonAsync<PaginatedCollection<AvailableGameRoomReadModel>>();
        paginatedReadModels.Should().NotBeNull();
        paginatedReadModels!.Items.Should().HaveCount(1);
        
        var room1 = paginatedReadModels.Items.Single();
        room1.GameRoomId.Should().Be(gameRoom.Id);
    }
    
    [Fact]
    public async Task ExcludesAlreadyFullGameRooms()
    {
        var gameRoom = new GameRoomBuilder()
            .WithIsPublic(true)
            .WithId(Guid.NewGuid())
            .WithCreator("player1")
            .WithPlayersThatJoined("player2", "player3")
            .Build();
        
        await SaveInRepository(
            gameRoom,
            new GameRoomBuilder()
                .WithIsPublic(true)
                .WithId(Guid.NewGuid())
                .WithCreator("player1")
                .WithPlayersThatJoined(Enumerable.Range(2, 9).Select(x => new PlayerId($"player{x}")).ToArray())
                .Build()
        );
        
        using var response = await Send(HttpMethod.Get, "game-rooms/available?pageNumber=1&pageSize=5", AnyPlayerId);
        await response.ShouldHaveSuccessStatusCode();
        
        var paginatedReadModels = await response.Content.ReadFromJsonAsync<PaginatedCollection<AvailableGameRoomReadModel>>();
        paginatedReadModels.Should().NotBeNull();
        paginatedReadModels!.Items.Should().HaveCount(1);
        
        var room1 = paginatedReadModels.Items.Single();
        room1.GameRoomId.Should().Be(gameRoom.Id);
    }
}