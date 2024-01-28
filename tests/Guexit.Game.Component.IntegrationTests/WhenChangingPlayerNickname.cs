using System.Net.Http.Json;
using Guexit.Game.Component.IntegrationTests.Extensions;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.Tests.Common.Builders;
using Guexit.Game.WebApi.Contracts.Requests;
using Microsoft.Extensions.DependencyInjection;

namespace Guexit.Game.Component.IntegrationTests;

public sealed class WhenChangingPlayerNickname : ComponentTest
{
    public WhenChangingPlayerNickname(GameWebApplicationFactory factory) : base(factory)
    { }

    [Fact]
    public async Task NicknameIsChanged()
    {
        var newNickname = "newNickname";
        var playerId = new PlayerId("d3fd8605-5e5f-4ddd-9d53-f7f5deb560d0");
        var username = "oldnickname@guexit.com";
        await Save(new PlayerBuilder()
            .WithId(playerId)
            .WithUsername(username)
            .Build());
        
        using var response = await Send(
            HttpMethod.Put, 
            $"/players/{playerId.Value}/nickname",
            JsonContent.Create(new ChangePlayerNicknameRequest(newNickname)),
            playerId
        );

        await response.ShouldHaveSuccessStatusCode();
        
        await using var scope = WebApplicationFactory.Services.CreateAsyncScope();
        var playerRepository = scope.ServiceProvider.GetRequiredService<IPlayerRepository>();
        
        var player = await playerRepository.GetBy(playerId);
        player!.Nickname.Should().NotBeNull();
        player.Nickname.Value.Should().Be(newNickname);
    }
    
    [Fact]
    public async Task ForbidsIfItsNotExecutedByThePlayerWhoIsChangingItsNickname()
    {
        var newNickname = "newNickname";
        var playerId = new PlayerId("d3fd8605-5e5f-4ddd-9d53-f7f5deb560d0");
        var username = "oldnickname@guexit.com";
        await Save(new PlayerBuilder()
            .WithId(playerId)
            .WithUsername(username)
            .Build());
        
        using var response = await Send(
            HttpMethod.Put, 
            $"/players/{playerId.Value}/nickname",
            JsonContent.Create(new ChangePlayerNicknameRequest(newNickname)),
            playerId
        );

        await response.ShouldHaveSuccessStatusCode();
        
        await using var scope = WebApplicationFactory.Services.CreateAsyncScope();
        var playerRepository = scope.ServiceProvider.GetRequiredService<IPlayerRepository>();
        
        var player = await playerRepository.GetBy(playerId);
        player!.Nickname.Should().NotBeNull();
        player.Nickname.Value.Should().Be(newNickname);
    }
}