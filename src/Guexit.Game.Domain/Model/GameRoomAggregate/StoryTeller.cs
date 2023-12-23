using Guexit.Game.Domain.Exceptions;
using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Domain.Model.GameRoomAggregate;

public sealed class StoryTeller : ValueObject
{
    public static readonly StoryTeller Empty = new(PlayerId.Empty, string.Empty);

    public PlayerId PlayerId { get; private init; } = default!;
    public string Story { get; private init; } = default!;

    public StoryTeller()
    {
        // EntityFramework required parameterless ctor
    }

    public StoryTeller(PlayerId playerId, string text)
    {
        PlayerId = playerId;
        Story = text;
    }

    public static StoryTeller Create(PlayerId playerId) => new(playerId, string.Empty);

    public StoryTeller SubmitStory(string story)
    {
        if (string.IsNullOrWhiteSpace(story))
            throw new EmptyCardStoryException();

        return new(PlayerId, story);
    }

    public bool HasSubmittedCardStory() => Story != string.Empty;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return PlayerId;
        yield return Story;
    }
}
