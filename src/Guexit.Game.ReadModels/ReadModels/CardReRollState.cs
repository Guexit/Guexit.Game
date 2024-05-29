using Guexit.Game.Domain.Model.GameRoomAggregate;

namespace Guexit.Game.ReadModels.ReadModels;

public sealed class CardReRollState
{
    public static readonly CardReRollState Empty = new("Empty");
    public static readonly CardReRollState Available = new("Available");
    public static readonly CardReRollState Completed = new("Completed");
    
    public string Value { get; }

    private CardReRollState(string value)
    {
        Value = value;
    }

    public static CardReRollState From(CardReRoll? reRoll) => reRoll switch
    {
        { IsCompleted: true } => Completed,
        { IsCompleted: false } => Available,
        _ => Empty,
    };
}