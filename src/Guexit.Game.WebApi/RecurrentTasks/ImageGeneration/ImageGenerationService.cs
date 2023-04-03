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
        ("A serene lakeside landscape at sunset, photorealistic, vibrant colors, intricate reflections, realistic, detailed, volumetric light and shadow, hyper HD, octane render, unreal engine, insanely detailed and intricate, hyper-realistic, super detailed.",
        "A bland, uninteresting landscape with dull colors."),
        ("An underwater coral reef scene, full of life and color, cinematic lighting, photorealistic, intricate, realistic, detailed, volumetric light and shadow, hyper HD, octane render, unreal engine, insanely detailed and intricate, hyper-realistic, super detailed.",
        "A barren underwater environment with minimal life."),
        ("A majestic mountain range during sunrise, cinematic lighting, photorealistic, ornate, intricate, realistic, detailed, volumetric light and shadow, hyper HD, octane render, unreal engine, insanely detailed and intricate, hyper-realistic, super detailed.",
        "A flat, featureless landscape with no mountains."),
        ("A bustling cityscape during twilight, cinematic lighting, photorealistic, ornate, intricate, realistic, detailed, volumetric light and shadow, hyper HD, octane render, unreal engine, insanely detailed and intricate, hyper-realistic, super detailed.",
        "A deserted city with no activity or life."),
        ("A tranquil Japanese Zen garden with cherry blossoms, cinematic lighting, photorealistic, ornate, intricate, realistic, detailed, volumetric light and shadow, hyper HD, octane render, unreal engine, insanely detailed and intricate, hyper-realistic, super detailed.",
        "A messy, disorganized garden with no sense of tranquility."),
        ("A lush tropical rainforest teeming with life, cinematic lighting, photorealistic, ornate, intricate, realistic, detailed, volumetric light and shadow, hyper HD, octane render, unreal engine, insanely detailed and intricate, hyper-realistic, super detailed.",
        "A barren, lifeless forest with no greenery."),
        ("A futuristic cityscape with cutting-edge architecture, cinematic lighting, photorealistic, ornate, intricate, realistic, detailed, volumetric light and shadow, hyper HD, octane render, unreal engine, insanely detailed and intricate, hyper-realistic, super detailed.",
        "A dated, rundown city with old architecture."),
        ("A breathtaking view of the cosmos, featuring nebulas, planets, and stars, cinematic lighting, photorealistic, ornate, intricate, realistic, detailed, volumetric light and shadow, hyper HD, octane render, unreal engine, insanely detailed and intricate, hyper-realistic, super detailed.",
        "An empty, uninteresting view of space with no celestial bodies."),
        ("An idyllic European village nestled in rolling hills, cinematic lighting, photorealistic, ornate, intricate, realistic, detailed, volumetric light and shadow, hyper HD, octane render, unreal engine, insanely detailed and intricate, hyper-realistic, super detailed.",
        "A bland, monotonous village with no charm or character."),
        ("A mesmerizing aurora borealis display over a snowy landscape, cinematic lighting, photorealistic, ornate, intricate, realistic, detailed, volumetric light and shadow, hyper HD, octane render, unreal engine, insanely detailed and intricate, hyper-realistic, super detailed.",
        "A dark, featureless night sky with no aurora.")
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
        var availableImagesCount = await _dbContext.Images.CountAsync(x => x.GameRoomId == null, cancellationToken);

        _logger.LogInformation("Actual available images count {availableImagesCount}. Target pool size {targetPoolSize}",
            availableImagesCount, _options.Value.TargetAvailableImagePoolSize);

        if (availableImagesCount >= _options.Value.TargetAvailableImagePoolSize)
            return;

        _logger.LogInformation("Sending {commandName}...", nameof(GenerateImagesCommand));

        var prompts = Prompts[Random.Shared.Next(Prompts.Length)];
        await _bus.Send(new GenerateImagesCommand
        {
            TextToImage = new TextToImage
            {
                Prompt = new Prompt
                {
                    Positive = prompts.PositivePrompt,
                    Negative = prompts.NegativePrompt
                },
                NumImages = 1
            }
        }, cancellationToken);
    }
}
