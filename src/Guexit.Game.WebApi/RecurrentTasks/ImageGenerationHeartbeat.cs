using System.ComponentModel.DataAnnotations;
using Guexit.Game.Messages;
using MassTransit;
using Microsoft.Extensions.Options;

namespace Guexit.Game.WebApi.RecurrentTasks;

public sealed class ImageGenerationHeartbeat : BackgroundService
{
    private readonly IOptions<ImageGenerationHeartbeatOptions> _options;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly PeriodicTimer _timer;

    public ImageGenerationHeartbeat(IOptions<ImageGenerationHeartbeatOptions> options, IServiceScopeFactory scopeFactory)
    {
        _options = options;
        _scopeFactory = scopeFactory;
        _timer = new PeriodicTimer(TimeSpan.FromMilliseconds(_options.Value.HeartbeatPeriodInMilliseconds));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await _timer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested)
        {
            await using var scope = _scopeFactory.CreateAsyncScope();

            var bus = scope.ServiceProvider.GetRequiredService<IBus>();
            var sendEndpoint = await bus.GetSendEndpoint(new Uri("queue:guexit-cron-generate-image-command"));

            await sendEndpoint.Send(new GenerateImagesCommand
            {
                Prompt = new() 
                {
                    Negative = "Pollito fresco",
                    Positive = "Pollito fresquisimo!"
                },
                Quantity = 1,
            }, stoppingToken);
        }
    }
}

public sealed class ImageGenerationHeartbeatOptions
{
    public const string SectionName = "ImageGenerationHeartbeat";

    [Required]
    public int TargetAvailableImagePoolSize { get; init; }
    [Required]
    public int HeartbeatPeriodInMilliseconds { get; init; }
    [Required]
    public bool Enabled { get; init; }
}