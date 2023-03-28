namespace Guexit.Game.Messages;

public class GenerateImagesCommand
{
    public required TextToImage TextToImage { get; init; }
    public bool ReturnImages { get; init; } = false;
    public bool UploadImages { get; init; } = true;
}

public class TextToImage
{
    public required string ModelPath { get; init; }
    public required string ModelScheduler { get; init; }
    public required Prompt Prompt { get; init; }
    public int Height { get; init; } = 688;
    public int Width { get; init; } = 512;
    public int NumInferenceSteps { get; init; } = 50;
    public int NumImages { get; init; } = 2;
    public int Seed { get; init; } = 57857;
}

public class Prompt
{
    public required string Positive { get; init; }
    public required string Negative { get; init; }
    public required double GuidanceScale { get; init; } = 16.5;
}