﻿namespace Guexit.Game.Domain.Model.GameRoomAggregate;

public sealed class Card : Entity<CardId>
{
    public Uri Url { get; private set; }

    public Card(CardId id, Uri url)
    {
        Id = id;
        Url = url;
    }
}

public sealed class CardId : ValueObject
{
    public static readonly CardId Empty = new(Guid.Empty);

    public Guid Value { get; }

    public CardId(Guid value) => Value = value;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static implicit operator CardId(Guid value) => new(value);
    public static implicit operator Guid(CardId value) => value.Value;
}
