﻿using System.Net;
using System.Net.Http.Json;
using Guexit.Game.Component.IntegrationTests.Builders;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.ReadModels.ReadModels;
using Guexit.Game.Tests.Common;
using Guexit.Game.WebApi.Requests;

namespace Guexit.Game.Component.IntegrationTests;

public sealed class WhenStorytellerSubmitsCardStory : ComponentTest
{
    public WhenStorytellerSubmitsCardStory(GameWebApplicationFactory factory) : base(factory)
    { }


    [Fact]
    public async Task CardStoryIsSubmitted()
    {
        var gameRoomId = new GameRoomId(Guid.NewGuid());
        var storyTellerId = new PlayerId("storyTellerPlayerId");
        var playerId2 = new PlayerId("player2");
        var playerId3 = new PlayerId("player3");
        var gameRoom = GameRoomBuilder.CreateStarted(gameRoomId, storyTellerId, new[] { playerId2, playerId3 }).Build();
        await Save(gameRoom);
        await Save(new[]
        {
            new PlayerBuilder().WithId(storyTellerId).WithUsername("gamora").Build(),
            new PlayerBuilder().WithId(playerId2).WithUsername("starlord").Build(),
            new PlayerBuilder().WithId(playerId3).WithUsername("wroot").Build()
        });

        var selectedCardId = gameRoom.PlayerHands.Single(x => x.PlayerId == storyTellerId).Cards.First().Id;
        var submitCardStoryRequestBody = new SubmitCardStoryRequest(selectedCardId, "El tipico abuelo adolescente"); 

        using var client = WebApplicationFactory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Post, $"/game-rooms/{gameRoom.Id.Value}/submit-card-story")
        {
            Content = JsonContent.Create(submitCardStoryRequestBody)
        };
        request.AddPlayerIdHeader(storyTellerId);
        var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK, because: await response.Content.ReadAsStringAsync());
        
        var getBoardRequest = new HttpRequestMessage(HttpMethod.Get, $"/game-rooms/{gameRoom.Id.Value}/board");
        getBoardRequest.AddPlayerIdHeader(storyTellerId);
        var getBoardResponse = await client.SendAsync(request);

        var responseContent = await getBoardResponse.Content.ReadFromJsonAsync<GameBoardReadModel>();
        responseContent.Should().NotBeNull();
        responseContent!.CurrentStoryTeller.PlayerId.Should().Be(storyTellerId);
        responseContent.CurrentStoryTeller.Story.Should().BeEmpty();
        responseContent.CurrentStoryTeller.Username.Should().BeEmpty();
    }
}

public sealed class WhenQueryingGameBoard : ComponentTest
{
    public WhenQueryingGameBoard(GameWebApplicationFactory factory) : base(factory)
    { }

    [Fact]
    public async Task ReturnsGameBoardReadModel()
    {
        var gameRoomId = new GameRoomId(Guid.NewGuid());
        var playerId1 = new PlayerId("storyTellerId"); 
        var playerId2 = new PlayerId("player2"); 
        var playerId3 = new PlayerId("player3");
        var gameRoom = GameRoomBuilder.CreateStarted(gameRoomId, playerId1, new[] { playerId2, playerId3 }).Build();
        await Save(gameRoom);
        await Save(new[]
        {
            new PlayerBuilder().WithId(playerId1).WithUsername("gamora").Build(),
            new PlayerBuilder().WithId(playerId2).WithUsername("starlord").Build(),
            new PlayerBuilder().WithId(playerId3).WithUsername("wroot").Build()
        });

        using var client = WebApplicationFactory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Get, $"/game-rooms/{gameRoom.Id.Value}/board");
        request.AddPlayerIdHeader(playerId1);
        var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK, because: await response.Content.ReadAsStringAsync());
        var responseContent = await response.Content.ReadFromJsonAsync<GameBoardReadModel>();
        responseContent.Should().NotBeNull();
        responseContent!.GameRoomId.Should().Be(gameRoomId);
        responseContent.CurrentStoryTeller.PlayerId.Should().Be(playerId1);
        responseContent.CurrentStoryTeller.Story.Should().BeEmpty();
        responseContent.CurrentStoryTeller.Username.Should().BeEmpty();
        responseContent.PlayerHand.Should().NotBeEmpty();
        responseContent.PlayerHand.Should().BeEquivalentTo(gameRoom.PlayerHands.Single(x => x.PlayerId == playerId1).Cards
            .Select(x => new GameBoardReadModel.CardDto { Id = x.Id, Url = x.Url }));
        responseContent.IsCurrentUserStoryTeller.Should().BeTrue();
        responseContent.SelectedCard.Should().BeNull();
    }

    [Fact]
    public async Task ReturnsNotFoundIfGameRoomDoesNotExist()
    {
        var nonExistingGameRoomId = new GameRoomId(Guid.NewGuid());
        var playerId1 = new PlayerId("storyTellerId"); 
        await Save(new PlayerBuilder().WithId(playerId1).WithUsername("gamora").Build());

        using var client = WebApplicationFactory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Get, $"/game-rooms/{nonExistingGameRoomId.Value}/board");
        request.AddPlayerIdHeader(playerId1);
        var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound, because: await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task ReturnsIfCurrentUserIsNotTheCurrentStoryTeller()
    {
        var gameRoomId = new GameRoomId(Guid.NewGuid());
        var initialStoryTeller = new PlayerId("storyTellerId");
        var playerId2 = new PlayerId("player2");
        var playerId3 = new PlayerId("player3");
        var gameRoom = GameRoomBuilder.CreateStarted(gameRoomId, initialStoryTeller, new[] { playerId2, playerId3 }).Build();
        await Save(gameRoom);
        await Save(new[]
        {
            new PlayerBuilder().WithId(initialStoryTeller).WithUsername("gamora").Build(),
            new PlayerBuilder().WithId(playerId2).WithUsername("starlord").Build(),
            new PlayerBuilder().WithId(playerId3).WithUsername("wroot").Build()
        });

        using var client = WebApplicationFactory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Get, $"/game-rooms/{gameRoom.Id.Value}/board");
        request.AddPlayerIdHeader(playerId3);
        var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK, because: await response.Content.ReadAsStringAsync());
        var responseContent = await response.Content.ReadFromJsonAsync<GameBoardReadModel>();
        responseContent.Should().NotBeNull();
        responseContent!.IsCurrentUserStoryTeller.Should().BeFalse();
    }

    [Fact]
    public async Task ReturnsBadRequestIfGameIsNotInProgress()
    {
        var gameRoomId = new GameRoomId(Guid.NewGuid());
        var playerId1 = new PlayerId("storyTellerId");
        var playerId2 = new PlayerId("player2");
        var playerId3 = new PlayerId("player3");
        var gameRoom = new GameRoomBuilder()
            .WithId(gameRoomId)
            .WithCreator(playerId1).WithPlayersThatJoined(playerId2, playerId3)
            .WithMinRequiredPlayers(3)
            .Build();
        await Save(gameRoom);
        await Save(new[]
        {
            new PlayerBuilder().WithId(playerId1).WithUsername("gamora").Build(),
            new PlayerBuilder().WithId(playerId2).WithUsername("starlord").Build(),
            new PlayerBuilder().WithId(playerId3).WithUsername("wroot").Build()
        });

        using var client = WebApplicationFactory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Get, $"/game-rooms/{gameRoom.Id.Value}/board");
        request.AddPlayerIdHeader(playerId1);
        var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest, because: await response.Content.ReadAsStringAsync());
    }
}
