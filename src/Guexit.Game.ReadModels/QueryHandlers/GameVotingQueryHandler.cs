using Guexit.Game.Application.Exceptions;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.Persistence;
using Guexit.Game.ReadModels.Extensions;
using Guexit.Game.ReadModels.ReadModels;
using Guexit.Game.ReadModels.ReadOnlyRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Guexit.Game.ReadModels.QueryHandlers;

public sealed class GameVotingQuery : IQuery<VotingReadModel>
{
    public GameRoomId GameRoomId { get; }
    public PlayerId PlayerId { get; }

    public GameVotingQuery(Guid gameRoomId, string playerId)
    {
        GameRoomId = new GameRoomId(gameRoomId);
        PlayerId = new PlayerId(playerId);
    }
}

public sealed class GameVotingQueryHandler : IQueryHandler<GameVotingQuery, VotingReadModel>
{
    private readonly ReadOnlyGameRoomRepository _gameRoomRepository;
    private readonly ReadOnlyPlayersRepository _playersRepository;

    public GameVotingQueryHandler(ReadOnlyGameRoomRepository gameRoomRepository, ReadOnlyPlayersRepository playersRepository)
    {
        _gameRoomRepository = gameRoomRepository;
        _playersRepository = playersRepository;
    }

    public async ValueTask<VotingReadModel> Handle(GameVotingQuery query, CancellationToken ct)
    {
        var gameRoom = await _gameRoomRepository.GetBy(query.GameRoomId, ct);
        if (gameRoom is null)
            throw new GameRoomNotFoundException(query.GameRoomId);

        var playersInGameRoom = (await _playersRepository.GetBy(gameRoom.PlayerIds, ct)).ToDictionary(x => x.Id);
        
        var submittedCards = gameRoom.SubmittedCards
            .Select(x => new VotingReadModel.SubmittedCardDto { Id = x.Card.Id, Url = x.Card.Url, WasSubmittedByQueryingPlayer = x.PlayerId == query.PlayerId })
            .OrderBy(x => x.Id)
            .ToArray();
        var guessingPlayersIds = gameRoom.GetCurrentGuessingPlayerIds();
        var guessingPlayers = playersInGameRoom.Where(x => guessingPlayersIds.Contains(x.Key)).ToArray();
        var playerIdsWhoAlreadyVoted = gameRoom.SubmittedCards.SelectMany(x => x.Voters).ToHashSet();
        var storyTeller = playersInGameRoom[gameRoom.CurrentStoryTeller.PlayerId];
        var votedCard = gameRoom.SubmittedCards.FirstOrDefault(x => x.Voters.Contains(query.PlayerId));
        
        return new VotingReadModel
        {
            Cards = submittedCards,
            GuessingPlayers = guessingPlayers.Select(x => new VotingReadModel.VotingPlayerDto
                {
                    PlayerId = x.Value.Id,
                    Username = x.Value.Username,
                    Nickname = x.Value.Nickname.Value,
                    HasVotedAlready = playerIdsWhoAlreadyVoted.Contains(x.Key)
                }) 
                .ToArray(),
            CurrentUserHasAlreadyVoted = playerIdsWhoAlreadyVoted.Contains(query.PlayerId),
            IsCurrentUserStoryTeller = gameRoom.CurrentStoryTeller.PlayerId == query.PlayerId,
            CurrentStoryTeller = new StoryTellerDto                                                              
            {
                PlayerId = storyTeller.Id.Value,
                Username = storyTeller.Username,
                Nickname = storyTeller.Nickname.Value,
                Story = gameRoom.CurrentStoryTeller.Story
            },
            CurrentUserVotedCard = votedCard is not null 
                ? new VotingReadModel.VotedCardDto { Id = votedCard.Card.Id, Url = votedCard.Card.Url } 
                : null
        };
    }
}
