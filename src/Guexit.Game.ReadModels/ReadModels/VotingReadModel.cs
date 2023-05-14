namespace Guexit.Game.ReadModels.ReadModels;

public sealed class VotingReadModel
{
    public required CardDto[] Cards { get; init; }


    public sealed class CardDto
    {
        public required Guid Id { get; init; }
        public required Uri Url { get; init; }
        public required string[] Voters { get; init; }
    }
}