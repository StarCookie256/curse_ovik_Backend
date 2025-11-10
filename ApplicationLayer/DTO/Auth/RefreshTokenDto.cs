using System.ComponentModel.DataAnnotations;

namespace PerfumeryBackend.ApplicationLayer.DTO.Auth;

public record RefreshTokenDto(
    [Required] string RefreshToken
);

