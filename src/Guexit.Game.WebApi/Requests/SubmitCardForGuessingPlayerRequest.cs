using System.ComponentModel.DataAnnotations;

namespace Guexit.Game.WebApi.Requests;

public sealed record SubmitCardForGuessingPlayerRequest(
    [Required] Guid CardId);

