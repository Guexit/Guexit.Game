using System;
using Guexit.Game.Domain.Model.ImageAggregate;

namespace Guexit.Game.Domain.Model.GameRoomAggregate;

public sealed class Card : Entity<CardId>
{
    public Uri Url { get; private set; } = default!;
    public ImageId ImageId { get; private set; } = default!;

    public Card(CardId id, Uri url)
    {
        Id = id;
        Url = url;
    }
}
