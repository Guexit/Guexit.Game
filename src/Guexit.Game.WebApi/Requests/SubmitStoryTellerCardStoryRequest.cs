using System.ComponentModel.DataAnnotations;

namespace Guexit.Game.WebApi.Requests;

public sealed record SubmitStoryTellerCardStoryRequest(
    [Required] Guid CardId, 
    [Required] string Story);

