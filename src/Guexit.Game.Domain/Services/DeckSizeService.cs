using Guexit.Game.Domain.Model.GameRoomAggregate;

namespace Guexit.Game.Domain.Services;

public static class DeckSizeService
{
    public static int Calculate(int totalPlayers, int desiredRounds = 1)
    {
        if (desiredRounds <= 0) 
            throw new ArgumentException($"{desiredRounds} is an invalid desired rounds count. It must be a positive number", nameof(desiredRounds));

        if (totalPlayers <= 0)
            throw new ArgumentException($"{totalPlayers} is an invalid total players count. It must be a positive number", nameof(totalPlayers));

        int initiallyDealtCards = GameRoom.PlayerHandSize * totalPlayers;
        int cardsInDeckAfterInitialDealt = (totalPlayers - 1) * totalPlayers;

        if (desiredRounds > 1)
        {
            var additionalRounds = desiredRounds - 1;
            cardsInDeckAfterInitialDealt += (int)Math.Pow(totalPlayers, 2) * additionalRounds;
        }

        return initiallyDealtCards + cardsInDeckAfterInitialDealt;
    }
}