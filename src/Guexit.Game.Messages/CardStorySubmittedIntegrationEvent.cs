namespace Guexit.Game.Messages;

public sealed class CardStorySubmittedIntegrationEvent
{
    public Guid GameRoomId { get; init; }
    public Guid CardId { get; init; }
    public string StoryTellerId { get; init; } = default!;
    public string Story { get; init; } = default!;


    public CardStorySubmittedIntegrationEvent()
    {
    }

    public CardStorySubmittedIntegrationEvent(Guid gameRoomId, Guid cardId, string storyTellerId, string story)
    {
        GameRoomId = gameRoomId;
        CardId = cardId;
        StoryTellerId = storyTellerId;
        Story = story;
    }
}
