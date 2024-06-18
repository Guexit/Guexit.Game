using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.Tests.Common.Builders;

namespace Guexit.Game.Tests.Common.ObjectMothers;

public static class GameRoomObjectMother
{
    public static GameRoom Finished(GameRoomId id, PlayerId creator, PlayerId[] invitedPlayers)
    {
        var gameRoom = GameRoomBuilder.CreateStarted(id, creator, invitedPlayers).Build();
        
        ConductRounds(gameRoom, roundsToConduct: gameRoom.GetPlayerCount());

        return gameRoom;
    }

    public static GameRoom Finished(GameRoomId id, PlayerId creator, PlayerId[] invitedPlayers, GameRoomId nextGameRoomId)
    {
        var gameRoom = Finished(id, creator, invitedPlayers);
        gameRoom.LinkToNextGameRoom(nextGameRoomId);
        
        gameRoom.ClearDomainEvents();
        return gameRoom;
    }


    public static GameRoom OneVotePendingToFinish(GameRoomId id, PlayerId creator, PlayerId[] invitedPlayers)
    {
        var gameRoom = GameRoomBuilder.CreateStarted(id, creator, invitedPlayers).Build();
        
        ConductRounds(gameRoom, roundsToConduct: gameRoom.GetPlayerCount() - 1);
        ConductCurrentRoundLeavingOnePlayerWithNoVote(gameRoom);

        gameRoom.ClearDomainEvents();
        return gameRoom;
    }

    private static void ConductRounds(GameRoom gameRoom, int roundsToConduct)
    {
        for (int i = 0; i < roundsToConduct; i++)
        {
            var storyTeller = gameRoom.CurrentStoryTeller.PlayerId;
            var story = $"Round {i} story";
            var cardsByPlayerId = gameRoom.PlayerHands.ToDictionary(x => x.PlayerId, v => v.Cards.First());

            gameRoom.SubmitStory(storyTeller, cardsByPlayerId[storyTeller].Id, story);

            var guessingPlayerIds = gameRoom.GetCurrentGuessingPlayerIds();
            foreach (var guessingPlayer in guessingPlayerIds)
                gameRoom.SubmitGuessingPlayerCard(guessingPlayer, cardsByPlayerId[guessingPlayer].Id);

            foreach (var guessingPlayer in guessingPlayerIds)
            {
                var cardToVote = gameRoom.SubmittedCards.First(x => x.PlayerId != guessingPlayer).Card;
                gameRoom.VoteCard(guessingPlayer, cardToVote.Id);
            }
        }
    }

    private static void ConductCurrentRoundLeavingOnePlayerWithNoVote(GameRoom gameRoom)
    {
        gameRoom.SubmitStory(
            gameRoom.CurrentStoryTeller.PlayerId,
            gameRoom.PlayerHands.First(x => x.PlayerId == gameRoom.CurrentStoryTeller.PlayerId).Cards.First().Id,
            "Any story"
        );

        foreach (var playerId in gameRoom.GetCurrentGuessingPlayerIds())
        {
            var cardId = gameRoom.PlayerHands.First(x => x.PlayerId == playerId).Cards.First().Id;
            gameRoom.SubmitGuessingPlayerCard(playerId, cardId);
        }

        foreach (var playerId in gameRoom.GetCurrentGuessingPlayerIds().Take(gameRoom.GetCurrentGuessingPlayerIds().Count - 1))
        {
            var cardId = gameRoom.SubmittedCards.First(x => x.PlayerId != playerId).Card.Id;
            gameRoom.VoteCard(playerId, cardId);
        }
    }
}
