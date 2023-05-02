using Guexit.Game.Domain.Exceptions;
using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Domain.Model.GameRoomAggregate;

public sealed class StoryTeller : ValueObject
{
    public static readonly StoryTeller Empty = new(PlayerId.Empty, CardId.Empty, string.Empty);

    public PlayerId PlayerId { get; private init; } = default!;
    public CardId SelectedCardId { get; private init; } = default!;
    public string Story { get; private init; } = default!;

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

    public StoryTeller SubmitCardWithStory(Card card, string story)
    {
        if (string.IsNullOrWhiteSpace(story))
            throw new EmptyCardStoryException();

        return new(PlayerId, card.Id, story);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return PlayerId;
        yield return SelectedCardId;
        yield return Story;
    }

    public bool HasSubmittedCardStory() => SelectedCardId != CardId.Empty && Story != string.Empty;
}
