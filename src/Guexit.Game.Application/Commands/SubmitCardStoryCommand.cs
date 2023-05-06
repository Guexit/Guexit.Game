using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Application.Commands;

public sealed class SubmitCardStoryCommand : ICommand
{
    public PlayerId PlayerId { get; }
    public GameRoomId GameRoomId { get; }
    public CardId CardId { get; }
    public string Story { get; }

    public SubmitCardStoryCommand(string playerId, Guid gameRoomId, Guid cardId, string story)
    {
        PlayerId = new PlayerId(playerId);
        GameRoomId = new GameRoomId(gameRoomId);
        CardId = cardId;
        Story = story;
    }
}
