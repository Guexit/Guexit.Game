namespace Guexit.Game.Component.IntegrationTests.DataCleaners;

public interface ITestDataCleaner
{
    ValueTask Clean(GameWebApplicationFactory webApplicationFactory);
}