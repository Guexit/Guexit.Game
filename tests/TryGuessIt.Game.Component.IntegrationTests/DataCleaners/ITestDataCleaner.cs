namespace TryGuessIt.Game.Component.IntegrationTests.DataCleaners;

public interface ITestDataCleaner
{
    ValueTask Clean(GameWebApplicationFactory webApplicationFactory);
}