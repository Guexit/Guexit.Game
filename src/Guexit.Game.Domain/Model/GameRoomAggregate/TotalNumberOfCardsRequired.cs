namespace Guexit.Game.Domain.Model.GameRoomAggregate;

public sealed class TotalNumberOfCardsRequired : ValueObject
{
    public int DesiredRounds { get; }
    public int TotalPlayers { get; }
    public int RequiredCardCount { get; }

    public TotalNumberOfCardsRequired(int totalPlayers, int desiredRounds = 1)
    {
        if (desiredRounds is <= 0) 
            throw new ArgumentException($"{desiredRounds} is an invalid desired rounds count. It must be a positive number", nameof(desiredRounds));

        if (totalPlayers is <= 0)
            throw new ArgumentException($"{totalPlayers} is an invalid total players count. It must be a positive number", nameof(totalPlayers));

        int initiallyDealtCards = GameRoom.PlayerHandSize * totalPlayers;
        int cardsInDeckAfterInitialDealt = (totalPlayers - 1) * totalPlayers;

        if (desiredRounds is > 1)
        {
            var aditionalRounds = desiredRounds - 1;
            cardsInDeckAfterInitialDealt += (int)Math.Pow(totalPlayers, 2) * aditionalRounds;
        }

        RequiredCardCount = initiallyDealtCards + cardsInDeckAfterInitialDealt;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return DesiredRounds;
        yield return TotalPlayers;
        yield return RequiredCardCount;
    }
}
