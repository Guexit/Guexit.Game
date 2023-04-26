using Guexit.Game.Domain.Model.GameRoomAggregate;

namespace Guexit.Game.Tests.Common;

public class CardBuilder
{
    private CardId _id;
    private Uri _url;

    public CardBuilder()
    {
        _id = new(Guid.NewGuid());
        _url = new($"https://pablocompany.com/{_id}");
    }

    public Card Build() => new(_id, _url);

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
