using System.Text.Json.Serialization;

namespace Guexit.Game.Messages;

public sealed class GenerateImagesCommandWithStyles
{
    [JsonPropertyName("text_to_style")]
    public TextToStyle TextToImage { get; init; } = new();
}

public sealed class TextToStyle
{
    [JsonPropertyName("style")]
    public string Style { get; init; } = "general";
    
    [JsonPropertyName("num_images")]
    public int NumImages { get; init; } = 3;
}