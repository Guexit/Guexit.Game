using System.Text.Json.Serialization;

namespace Guexit.Game.Messages;

public sealed class GenerateImagesCommand
{
    [JsonPropertyName("text_to_image")]
    public required TextToImage TextToImage { get; init; }

    [JsonPropertyName("return_images")]
    public bool ReturnImages { get; init; } = false;

    [JsonPropertyName("upload_images")]
    public bool UploadImages { get; init; } = true;
}

public sealed class TextToImage
{
    [JsonPropertyName("model_path")]
    public string ModelPath { get; init; } = "prompthero/openjourney-v2";

    [JsonPropertyName("model_scheduler")]
    public string ModelScheduler { get; init; } = "euler_a";

    [JsonPropertyName("prompt")]
    public required Prompt Prompt { get; init; }

    [JsonPropertyName("height")]
    public int Height { get; init; } = 688;

    [JsonPropertyName("width")]
    public int Width { get; init; } = 512;

    [JsonPropertyName("num_inference_steps")]
    public int NumInferenceSteps { get; init; } = 50;

    [JsonPropertyName("num_images")]
    public required int NumImages { get; init; }

    [JsonPropertyName("seed")]
    public int Seed { get; init; } = 57857;
}

public sealed class Prompt
{
    [JsonPropertyName("positive")]
    public required string Positive { get; init; }

    [JsonPropertyName("negative")]
    public required string Negative { get; init; }

    [JsonPropertyName("guidance_scale")]
    public double GuidanceScale { get; init; } = 16.5;
}