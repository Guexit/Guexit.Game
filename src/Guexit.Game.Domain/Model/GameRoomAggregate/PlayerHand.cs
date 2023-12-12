using System.Security.Cryptography.X509Certificates;
using Guexit.Game.Domain.Exceptions;
using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Domain.Model.GameRoomAggregate;

public sealed class PlayerHand : Entity<PlayerHandId>
{
    public PlayerId PlayerId { get; private set; } = default!;
    public ICollection<Card> Cards { get; private set; } = new List<Card>();
    public GameRoomId GameRoomId { get; private set; } = default!;

    private PlayerHand()
    {
        // Entity Framework required parameterless ctor 
    }

    public PlayerHand(PlayerHandId id, PlayerId playerId, List<Card> cards, GameRoomId gameRoomId)
    {
        Id = id;
        PlayerId = playerId;
        Cards = cards;
        GameRoomId = gameRoomId;
    }

    public void AddCard(Card card)
    {
        Cards.Add(card);
    }

    public Card SubtractCard(CardId cardId)
    {
        var card = GetCard(cardId);
        RemoveCard(cardId);
        return card;
    }

    private void RemoveCard(CardId cardId) => Cards.Remove(GetCard(cardId));

    private Card GetCard(CardId cardId)
    {
        return Cards.SingleOrDefault(x => x.Id == cardId) 
            ?? throw new CardNotFoundInPlayerHandException(PlayerId, cardId);
    }
}

public sealed class PlayerHandId : ValueObject
{
    public static readonly PlayerHandId Empty = new(Guid.Empty);

    public Guid Value { get; }

    public PlayerHandId(Guid value)
    {
        Value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
