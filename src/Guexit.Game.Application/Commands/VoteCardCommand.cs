using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Application.Commands;

public sealed class VoteCardCommand : IGameRoomCommand
{
    public PlayerId VotingPlayerId { get; }
    public GameRoomId GameRoomId { get; }
    public CardId SubmittedCardId { get; }

    public VoteCardCommand(string playerId, Guid gameRoomId, Guid submittedCardId)
    {
        VotingPlayerId = new PlayerId(playerId);
        GameRoomId = new GameRoomId(gameRoomId);
        SubmittedCardId = submittedCardId;
    }
}