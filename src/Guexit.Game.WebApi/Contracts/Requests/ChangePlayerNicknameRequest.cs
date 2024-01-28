using System.ComponentModel.DataAnnotations;

namespace Guexit.Game.WebApi.Contracts.Requests;

public sealed record ChangePlayerNicknameRequest([Required] string Nickname);