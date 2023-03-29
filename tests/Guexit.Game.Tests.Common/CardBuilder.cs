using Guexit.Game.Domain.Model.GameRoomAggregate;

namespace Guexit.Game.Tests.Common;

public class CardBuilder
{
    private CardId _id = new(Guid.NewGuid());
    private Uri _url = new("https://pablocompany.com");

    public Card Build() => new Card(_id, _url);

    public CardBuilder WithId(CardId id)
    {
        _id = id;
        return this;
    }

    public CardBuilder WithUrl(Uri url)
    {
        _url = url;
        return this;
    }

}
