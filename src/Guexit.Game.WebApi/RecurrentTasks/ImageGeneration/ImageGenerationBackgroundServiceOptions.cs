using System.ComponentModel.DataAnnotations;

namespace Guexit.Game.WebApi.RecurrentTasks.ImageGeneration;

public sealed class ImageGenerationBackgroundServiceOptions
{
    public const string SectionName = "ImageGenerationBackgroundService";

    [Required]
    public int TargetAvailableImagePoolSize { get; init; }
    [Required]
    public int PeriodInMilliseconds { get; init; }
    [Required]
    public bool Enabled { get; init; }
}