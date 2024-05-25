using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.Tests.Common.Builders;

namespace Guexit.Game.Tests.Common.ObjectMothers;

public static class GameRoomObjectMother
{
    public static GameRoom Finished(GameRoomId id, PlayerId creator, PlayerId[] invitedPlayers)
    {
        var gameRoom = new GameRoom(id, creator, new DateTimeOffset(2024, 1, 1, 1, 2, 3, TimeSpan.Zero));

        foreach (var player in invitedPlayers)
            gameRoom.Join(player);

        var cards = BuildCards(gameRoom);

        gameRoom.AssignDeck(cards);
        gameRoom.Start(creator);
        ConductRounds(gameRoom, roundsToConduct: gameRoom.GetPlayersCount());

        return gameRoom;
    }

    public static GameRoom Finished(GameRoomId id, PlayerId creator, PlayerId[] invitedPlayers, GameRoomId nextGameRoomId)
    {
        var gameRoom = Finished(id, creator, invitedPlayers);
        gameRoom.LinkToNextGameRoom(nextGameRoomId);

        return gameRoom;
    }


    public static GameRoom OneVotePendingToFinish(GameRoomId id, PlayerId creator, PlayerId[] invitedPlayers)
    {
        var gameRoom = GameRoomBuilder.CreateStarted(id, creator, invitedPlayers).Build();
        
        ConductRounds(gameRoom, roundsToConduct: gameRoom.GetPlayersCount() - 1);
        ConductCurrentRoundLeavingOnePlayerWithNoVote(gameRoom);

        return gameRoom;
    }
    
    public static GameRoom OneVotePendingToCompleteFirstRound(GameRoomId id, PlayerId creator, PlayerId[] invitedPlayers)
    {
        var gameRoom = GameRoomBuilder.CreateStarted(id, creator, invitedPlayers).Build();
        
        ConductCurrentRoundLeavingOnePlayerWithNoVote(gameRoom);

        return gameRoom;
    }

    private static Card[] BuildCards(GameRoom gameRoom)
    {
        return Enumerable.Range(0, gameRoom.GetRequiredNumberOfCardsInDeck())
            .Select((_, i) => new CardBuilder().WithUrl($"https://guexit.com/images/{i}").Build())
            .ToArray();
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
