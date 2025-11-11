using System.ComponentModel.DataAnnotations;

namespace PerfumeryBackend.MainLayer.Contracts.Auth;

public record RefreshTokenRequest(
    [Required] string RefreshToken
);
