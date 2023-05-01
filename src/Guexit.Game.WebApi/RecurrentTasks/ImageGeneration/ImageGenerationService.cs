using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Messages;
using Guexit.Game.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Guexit.Game.WebApi.RecurrentTasks.ImageGeneration;

public sealed class ImageGenerationService
{
    private static readonly (string PositivePrompt, string NegativePrompt)[] Prompts = new[]
    {
        ("A giraffe wearing a top hat and monocle, playing a grand piano in a lush, sunlit meadow.",
        "An empty meadow with no animals, objects, or instruments."),
        ("A squirrel wearing a cape, conducting a symphony orchestra of various animals in a vibrant forest.",
        "A quiet forest with no animals, objects, or instruments."),
        ("An elephant using its trunk to paint an abstract masterpiece on a canvas, surrounded by a group of admiring animals.",
        "An empty room with blank walls and no animals, objects, or instruments."),
        ("A kangaroo playing the guitar while standing on its tail, performing a lively concert for an audience of enthusiastic animals.",
        "A silent, deserted stage with no animals, objects, or instruments."),
        ("A raccoon wearing a chef's hat, expertly preparing a gourmet meal using various kitchen utensils in a bustling animal café.",
        "An empty, unoccupied kitchen with no animals, objects, or instruments."),
        ("A group of animals dressed in elaborate costumes, performing a strange and whimsical play using bizarre props and instruments.",
        "An empty theater with no animals, objects, or instruments."),
        ("A monkey dressed as a scientist, using intricate contraptions and devices to create a groundbreaking invention in a jungle laboratory.",
        "An empty lab with no animals, objects, or instruments."),
        ("A penguin in a spacesuit, floating through a vibrant cosmic landscape, playing a saxophone that emits a trail of glowing stardust.",
        "An empty, uninteresting view of space with no animals, objects, or instruments."),
        ("A llama wearing a bowtie, serving as a master of ceremonies at a grand animal ball, where various creatures dance and socialize with elegant attire.",
        "An empty, silent ballroom with no animals, objects, or instruments."),
        ("A group of animals ice-skating on a frozen lake, using various instruments to create an impromptu winter symphony under the dazzling aurora borealis display.",
        "A frozen, deserted lake with no animals, objects, or instruments, and a featureless night sky.")
    };

    private readonly IBus _bus;
    private readonly GameDbContext _dbContext;
    private readonly ILogger<ImageGenerationService> _logger;
    private readonly IOptions<ImageGenerationBackgroundServiceOptions> _options;

    public ImageGenerationService(IBus bus, GameDbContext dbContext, ILogger<ImageGenerationService> logger,
        IOptions<ImageGenerationBackgroundServiceOptions> options)
    {
        (_bus, _dbContext, _logger, _options) = (bus, dbContext, logger, options);
    }

    public async Task GenerateImages(CancellationToken cancellationToken = default)
    {
        var availableImagesCount = await _dbContext.Images.CountAsync(x => x.GameRoomId == GameRoomId.Empty, cancellationToken);

        _logger.LogInformation("Actual available images count {availableImagesCount}. Target pool size {targetPoolSize}",
            availableImagesCount, _options.Value.TargetAvailableImagePoolSize);

        if (availableImagesCount >= _options.Value.TargetAvailableImagePoolSize)
            return;

        _logger.LogInformation("Sending {commandName}...", nameof(GenerateImagesCommand));

        var prompt = Prompts[Random.Shared.Next(Prompts.Length)];
        await _bus.Send(new GenerateImagesCommand
        {
            TextToImage = new TextToImage
            {
                Prompt = new Prompt
                {
                    Positive = prompt.PositivePrompt,
                    Negative = prompt.NegativePrompt
                },
                NumImages = 1,
                Seed = -1
            }
        }, cancellationToken);
    }
}
