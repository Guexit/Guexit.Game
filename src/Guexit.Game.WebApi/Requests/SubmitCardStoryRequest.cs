namespace Guexit.Game.WebApi.Requests;

public sealed record SubmitCardStoryRequest(Guid CardId, string Story);
