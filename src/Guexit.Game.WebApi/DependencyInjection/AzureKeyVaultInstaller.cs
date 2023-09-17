using Azure.Identity;

namespace Guexit.Game.WebApi.DependencyInjection;

public static class AzureKeyVaultInstaller
{
    public static void AddAzureKeyVault(this ConfigurationManager configuration)
    {
        var options = configuration.GetSection(AzureKeyVaultOptions.SectionName).Get<AzureKeyVaultOptions>();
        if (options is null)
            throw new Exception("Cannot retrieve Azure KeyVault options from configuration");

        if (!options.Enabled)
            return;

        configuration.AddAzureKeyVault(new Uri(options.Uri), new DefaultAzureCredential());
    }
}

public sealed class AzureKeyVaultOptions
{
    public const string SectionName = "AzureKeyVault";
    
    public required bool Enabled { get; init; }
    public required string Uri { get; init; }
}
