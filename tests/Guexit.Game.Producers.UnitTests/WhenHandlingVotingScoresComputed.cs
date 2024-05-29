using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.GameRoomAggregate.Events;
using Guexit.Game.Messages;
using MassTransit;
using NSubstitute;

namespace Guexit.Game.Producers.UnitTests;

public sealed class WhenHandlingVotingScoresComputed
{
    private readonly VotingScoresComputedProducer _eventHandler;
    private readonly IPublishEndpoint _publishEndpoint;

    public WhenHandlingVotingScoresComputed()
    {
        _publishEndpoint = Substitute.For<IPublishEndpoint>();
        _eventHandler = new VotingScoresComputedProducer(_publishEndpoint);
    }

    [Fact]
    public async Task VotingScoresComputedIntegrationEventIsPublished()
    {
        var gameRoomId = new GameRoomId(Guid.NewGuid());
        var gameStarted = new VotingScoresComputed(gameRoomId);

        await _eventHandler.Handle(gameStarted);

        await _publishEndpoint.Received(1).Publish(Arg.Is<VotingScoresComputedIntegrationEvent>(e => e.GameRoomId == gameRoomId));
    }
}
