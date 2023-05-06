using System.ComponentModel.DataAnnotations;

namespace Guexit.Game.WebApi.RecurrentTasks.ImageGeneration;

public sealed class ImageGenerationBackgroundServiceOptions
{
    public const string SectionName = "ImageGenerationRecurrentTask";

    [Required]
    public int TargetAvailableImagePoolSize { get; init; }
    [Required]
    [Range(0, 100000)]
    public int PeriodInSeconds { get; init; }
    [Required]
    public bool Enabled { get; init; }
}