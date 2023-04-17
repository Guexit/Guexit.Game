using Microsoft.Extensions.Options;

namespace Guexit.Game.WebApi.RecurrentTasks.ImageGeneration;

public sealed class ImageGenerationBackgroundService : BackgroundService
{
    private readonly IOptions<ImageGenerationBackgroundServiceOptions> _options;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly PeriodicTimer _timer;

    public ImageGenerationBackgroundService(IOptions<ImageGenerationBackgroundServiceOptions> options, IServiceScopeFactory scopeFactory)
    {
        _options = options;
        _scopeFactory = scopeFactory;
        _timer = new PeriodicTimer(TimeSpan.FromMilliseconds(_options.Value.PeriodInMilliseconds));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await _timer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested)
        {
            if (!_options.Value.Enabled)
                return;

            await using var scope = _scopeFactory.CreateAsyncScope();
            var imageGenerationService = scope.ServiceProvider.GetRequiredService<ImageGenerationService>();

            await imageGenerationService.GenerateImages(stoppingToken);
        }
    }
}
