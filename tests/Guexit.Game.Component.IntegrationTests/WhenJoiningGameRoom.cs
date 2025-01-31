﻿using System.Net;
using Guexit.Game.Component.IntegrationTests.Builders;
using Guexit.Game.Component.IntegrationTests.Extensions;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.Persistence;
using Guexit.Game.Tests.Common;
using Guexit.Game.Tests.Common.Builders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Guexit.Game.Component.IntegrationTests;

public sealed class WhenJoiningGameRoom : ComponentTest
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public WhenJoiningGameRoom(GameWebApplicationFactory factory)
        : base(factory)
    {
        _serviceScopeFactory = factory.Services.GetRequiredService<IServiceScopeFactory>();
    }

    [Fact]
    public async Task PlayerIsAddedToGame()
    {
        var creatorId = new PlayerId("player1");
        var playerJoiningId = new PlayerId("player2");
        var gameRoomId = new GameRoomId(Guid.NewGuid());
        await SaveInRepository(new PlayerBuilder().WithId(playerJoiningId).Build());
        await SaveInRepository(new GameRoom(gameRoomId, creatorId, new DateTimeOffset(2023, 1, 1, 2, 3, 4, TimeSpan.Zero)));

        using var response = await Send(HttpMethod.Post, $"game-rooms/{gameRoomId.Value}/join", playerJoiningId);

        response.Should().NotBeNull();
        await response.ShouldHaveSuccessStatusCode();

        await AssertGameRoomHasPlayers(creatorId, playerJoiningId);
    }

    [Fact]
    public async Task ReturnsOkEvenIfPlayerIsAlreadyInGameRoom()
    {
        var creatorId = new PlayerId("player1");
        var gameRoomId = new GameRoomId(Guid.NewGuid());
        await SaveInRepository(new PlayerBuilder().WithId(creatorId).Build());
        await SaveInRepository(new GameRoom(gameRoomId, creatorId, new DateTimeOffset(2023, 1, 1, 2, 3, 4, TimeSpan.Zero)));

        using var response = await Send(HttpMethod.Post, $"game-rooms/{gameRoomId.Value}/join", creatorId);

        await response.ShouldHaveSuccessStatusCode();
    }

    private async Task AssertGameRoomHasPlayers(PlayerId creator, PlayerId playerJoining)
    {
        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<GameDbContext>();
        var gameRooms = await dbContext.GameRooms.ToArrayAsync();

        gameRooms.Should().HaveCount(1);
        gameRooms[0].PlayerIds.Should().BeEquivalentTo(new[] { creator, playerJoining });
        gameRooms[0].RequiredMinPlayers.Should().Be(RequiredMinPlayers.Default);
    }
}
