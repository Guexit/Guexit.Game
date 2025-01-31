﻿using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Domain.Model.GameRoomAggregate.Events;

public sealed class StoryTellerCardStorySubmitted : IDomainEvent
{
    public Guid GameRoomId { get; }
    public string StoryTellerId { get; }
    public Guid SelectedCardId { get; }
    public string Story { get; }

    public StoryTellerCardStorySubmitted(GameRoomId gameRoomId, PlayerId storyTellerId, CardId cardId, string story)
    {
        GameRoomId = gameRoomId.Value;
        StoryTellerId = storyTellerId.Value;
        SelectedCardId = cardId.Value;
        Story = story;
    }
}
