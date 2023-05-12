namespace Guexit.Game.Messages;

/// <summary>
/// This event comes from ImageGeneration service and it is raised just after an image is generated
/// <!--
/// Important: DO NOT move to a different namespace or change name of the class,
/// it's used by masstransit to match registered urn type name
/// -->
/// </summary>
/// <param name="Url">Url where image can be downloaded</param>
public sealed record ImageGenerated(string Url);