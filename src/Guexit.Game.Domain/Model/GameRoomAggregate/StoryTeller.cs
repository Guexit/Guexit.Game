using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Domain.Model.GameRoomAggregate;

public sealed class StoryTeller : ValueObject
{
    public static readonly StoryTeller Empty = new(PlayerId.Empty, CardId.Empty, string.Empty);

    public PlayerId PlayerId { get; private set; } = default!;
    public CardId SelectedCardId { get; private set; } = default!;
    public string Story { get; private set; } = default!;

    public StoryTeller()
    {
        // EntityFramework required parameterless ctor
    }

    public StoryTeller(PlayerId playerId, CardId selectedCardId, string text)
    {
        PlayerId = playerId;
        SelectedCardId = selectedCardId;
        Story = text;
    }

    public static StoryTeller Create(PlayerId playerId) => new(playerId, CardId.Empty, string.Empty);

    public StoryTeller SubmitCardWithStory(CardId cardId, string story)
    {
        if (string.IsNullOrWhiteSpace(story))
        { 
            // Todo: throw exception
        }

        return new(PlayerId, cardId, story);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return PlayerId;
        yield return SelectedCardId;
        yield return Story;
    }
}
