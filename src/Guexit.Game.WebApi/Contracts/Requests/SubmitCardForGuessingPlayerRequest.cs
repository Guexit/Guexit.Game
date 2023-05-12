using System.ComponentModel.DataAnnotations;

namespace Guexit.Game.WebApi.Contracts.Requests;

public sealed record SubmitCardForGuessingPlayerRequest(
    [Required] Guid CardId);

