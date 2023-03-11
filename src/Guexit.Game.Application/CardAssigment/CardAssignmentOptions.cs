using System.ComponentModel.DataAnnotations;

namespace Guexit.Game.Application.CardAssigment;

public sealed class CardAssignmentOptions
{
    public const string SectionName = "CardAssignment";

    [Required]
    [Range(1, 10)]
    public int LogicalShardsCount { get; init; } = 1;
}
